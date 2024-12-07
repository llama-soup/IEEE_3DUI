using UnityEngine;
using Unity.Netcode;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using System;
using TMPro;
using UnityEngine.UI;
using OpenAI;
using System.Collections.Generic;
using Samples.Whisper;

public class NetworkPlayer : NetworkBehaviour
{
    public Material presentSkybox;
    public Material futureSkybox;
    private Color presentFogColor = Color.blue;
    private Color futureFogcolor = Color.gray;
    const  float presentFogDensity = 0.02f;
    const float futureFogDensity = 0.04f;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public GameObject futurePlayerSpawnPoint;

    public Renderer[] meshToDisable;
    //Traslate variables
    private GameObject languageSetting;
    private GameObject microphones;
    public String language;
    public String microphone;
    public int player_ID;
    public TextMeshProUGUI messageDisplay;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "Act as a translator for the user. Your response should only include the translation requested. Don't break character. Don't ever mention that you are an AI model.";
    
    private readonly string fileName = "output.wav";
    private readonly int duration = 10;
    
    private AudioClip clip;
    private bool isRecording;
    private bool stop;
    private float time;
    private OpenAIApi openai = new OpenAIApi();
    //end of translate variables
    private NetworkVariable<Vector3> netRootPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> netRootRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> netHeadLocalPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> netHeadLocalRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> netLeftHandLocalPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> netLeftHandLocalRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> netRightHandLocalPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> netRightHandLocalRotation = new NetworkVariable<Quaternion>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                if (item != null) item.enabled = false;
            }
        }
        if (IsClient){
            UpdateClientEnvironment();
            player_ID = NetworkManager.Singleton.ConnectedClients.Count;
            CreateMessageBox();
        }
        //Boolean that stops the audio recording.
        stop = false;

    }
    //Creates the message dissplay for the player
    void CreateMessageBox(){
        // Create a new GameObject for the text
        GameObject textObject = new GameObject("Translation Text");

        // Attach the TextMeshPro component
        TextMeshProUGUI messageDisplay = textObject.AddComponent<TextMeshProUGUI>();

        // Set the text properties
        messageDisplay.text = "";
        messageDisplay.fontSize = 36;
        messageDisplay.alignment = TextAlignmentOptions.Center;

        // Get or create a Canvas in the scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        // Set the text object's parent to the canvas
        textObject.transform.SetParent(canvas.transform, false);

        // Position the text at the bottom middle of the screen
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);
        rectTransform.pivot = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, 50); // Adjust the Y value for padding
    }
    //Updates the language to what the player selects in the menu
    void UpdateLanguage(){
        if (languageSetting.TryGetComponent(out TMP_Dropdown component))
        {
            int pickedIndex = component.value;
            language = component.options[pickedIndex].text;
        }
        else
        {
            // Component was not found
            Debug.Log("Component not found!");
        }
    }
    //Updates the microphone to the one selected in the menu
    void UpdateMicrophone(){
        if (microphones.TryGetComponent(out TMP_Dropdown component))
        {
            int pickedIndex = component.value;
            microphone = component.options[pickedIndex].text;
        }
        else
        {
            // Component was not found
            Debug.Log("Component not found!");
        }
    }
    //Function called by the input manager to start translation
    public void SpaceRecord()
    {
        if(IsOwner)
        {
            isRecording = true;
            #if !UNITY_WEBGL
            clip = Microphone.Start(microphone, false, duration, 44100);
            SendReply();
            #endif
        }
    }
    //Function called by input manager to end translation.
    public void EndRecord()
    {
        if(IsOwner)
        {
            stop = true;
        }
    }
    //Translates the message and sends it to the display
    private async void SendReply()
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = messageDisplay.text
        };

        //Makes the prompt
        if (messages.Count == 0) 
        {
            if(language == "Select a Language"){
                language = "English";
            }
            newMessage.Content = prompt + " Translate the message from English to " + language + ".\n" + messageDisplay.text;
            //Debug.Log(newMessage.Content);
        }
        
        messages.Add(newMessage);
        
        messageDisplay.text = "";
        
        //translates the message
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-4o-mini",
            Messages = messages
        });
        //Moves message to display
        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();
            
            messageDisplay.text = message.Content;
            messages.Clear();
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }
    }
    //Ends the recording and performs the audio to text transformation
    private async void EndRecording()
    {
        messageDisplay.text = "Transcripting...";
        
        #if !UNITY_WEBGL
        Microphone.End(null);
        #endif
        
        byte[] data = SaveWav.Save(fileName, clip);
        
        //Audio to text
        var req = new CreateAudioTranslationRequest
        {
            FileData = new FileData() {Data = data, Name = "audio.wav"},
            // File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
        };
        var res = await openai.CreateAudioTranslation(req);

        messageDisplay.gameObject.SetActive(false);
        messageDisplay.text = res.Text;
        SendReply();
        messageDisplay.gameObject.SetActive(true);
    }

    void UpdateClientEnvironment(){
        // Set HDR to not cloudy day if we are the client (aka not the server, player 1)(aka player 2)
        RenderSettings.fogColor = futureFogcolor;
        RenderSettings.fogDensity = futureFogDensity;
        RenderSettings.skybox = futureSkybox;
        DynamicGI.UpdateEnvironment();
    }

    void SetClientPos(GameObject playerToMove){
        playerToMove.transform.position = futurePlayerSpawnPoint.transform.position;
    }

    void Update()
    {
        if (IsOwner)
        {
            UpdateLocalTransforms();
            SyncPositionsServerRpc();
        }
        else
        {
            UpdateNetworkTransforms();
        }
        //Checks if the language menu is open and if so updates the language
        if (GameObject.FindWithTag("Language Options") != null)
        {
            languageSetting = GameObject.FindWithTag("Language Options");
            UpdateLanguage();
        }
        //Checks if the microphone menu is open and if so updates the microphone
        if (GameObject.FindWithTag("Microphone Dropdown") != null)
        {
            microphones = GameObject.FindWithTag("Microphone Dropdown");
            UpdateMicrophone();
        }
        //Process for recording the messages
        if (isRecording)
        {
            time += Time.deltaTime;
            
            if (time >= duration || stop)
            {
                time = 0;
                isRecording = false;
                stop = false;
                EndRecording();
            }
        }
    }

    void UpdateLocalTransforms()
    {
        if (VRRigReferences.Singleton != null)
        {
            transform.position = VRRigReferences.Singleton.root.position;
            transform.rotation = VRRigReferences.Singleton.root.rotation;

            head.localPosition = VRRigReferences.Singleton.head.localPosition;
            head.localRotation = VRRigReferences.Singleton.head.localRotation;
            leftHand.localPosition = VRRigReferences.Singleton.leftHand.localPosition;
            leftHand.localRotation = VRRigReferences.Singleton.leftHand.localRotation;
            rightHand.localPosition = VRRigReferences.Singleton.rightHand.localPosition;
            rightHand.localRotation = VRRigReferences.Singleton.rightHand.localRotation;
        }
    }

    void UpdateNetworkTransforms()
    {
        transform.position = netRootPosition.Value;
        transform.rotation = netRootRotation.Value;

        head.localPosition = netHeadLocalPosition.Value;
        head.localRotation = netHeadLocalRotation.Value;
        leftHand.localPosition = netLeftHandLocalPosition.Value;
        leftHand.localRotation = netLeftHandLocalRotation.Value;
        rightHand.localPosition = netRightHandLocalPosition.Value;
        rightHand.localRotation = netRightHandLocalRotation.Value;
    }

    [ServerRpc]
    void SyncPositionsServerRpc()
    {
        netRootPosition.Value = transform.position;
        netRootRotation.Value = transform.rotation;

        netHeadLocalPosition.Value = head.localPosition;
        netHeadLocalRotation.Value = head.localRotation;
        netLeftHandLocalPosition.Value = leftHand.localPosition;
        netLeftHandLocalRotation.Value = leftHand.localRotation;
        netRightHandLocalPosition.Value = rightHand.localPosition;
        netRightHandLocalRotation.Value = rightHand.localRotation;
    }
}