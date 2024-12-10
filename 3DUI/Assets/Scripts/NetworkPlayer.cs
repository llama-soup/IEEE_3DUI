using UnityEngine;
using Unity.Netcode;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using System;
using TMPro;
using UnityEngine.UI;
using OpenAI;
using System.Collections.Generic;
using Samples.Whisper;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Hands.OpenXR;
using Unity.VisualScripting;
using JetBrains.Annotations;
using Unity.XR.CoreUtils;

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

    public GameObject presentPlayerSpawnPoint;

    public Renderer[] meshToDisable;
    //Traslate variables
    private GameObject languageSetting;
    private GameObject microphones;
    public String language;
    public String microphone;
    public int player_ID;
    private string messageTemp;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "Act as a translator for the user. Your response should only include the translation requested. Don't break character. Don't ever mention that you are an AI model.";
    
    private readonly string fileName = "output.wav";
    private readonly int duration = 10;
    
    private AudioClip clip;
    private bool isRecording;

    public XROrigin xrOrigin;
    private bool stop;
    private float time;
    private OpenAIApi openai = new OpenAIApi();
    private GameObject mainText;
    private ChatBox mainChatScript;
    public GameObject canvasPrefab; // Assign this in the Inspector
    private GameObject playerCanvas;
    private TMP_Text playerChat;
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

            player_ID = NetworkManager.Singleton.ConnectedClients.Count;
            language = "english";
            //Boolean that stops the audio recording.
            stop = false;
            playerCanvas = Instantiate(canvasPrefab, transform);
            playerCanvas.transform.localPosition = new Vector3(0, 1.5f, 2.0f); // Adjust position relative to the player
            playerChat = playerCanvas.GetComponentInChildren<TMP_Text>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {




        if(!IsServer){
            UpdateClientEnvironment();
            SetClientPos();
        }

        IntializeMainText();
    }

    private void IntializeMainText(){
        mainText = GameObject.FindWithTag("Chat");
        Debug.Log(mainText);
        mainChatScript = mainText.GetComponent<ChatBox>();
        Debug.Log(mainChatScript);
        mainChatScript.Updatelanguage(player_ID, language);
    }

    //Creates the message dissplay for the player
    
    //Updates the language to what the player selects in the menu
    void UpdateLanguage(){
        if (languageSetting.TryGetComponent(out TMP_Dropdown component))
        {
            int pickedIndex = component.value;
            language = component.options[pickedIndex].text;
            mainChatScript.Updatelanguage(player_ID, language);
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
            Debug.Log(microphone);
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
            Content = messageTemp
        };

        //Makes the prompt
        if (messages.Count == 0) 
        {
            if(player_ID == 0){
                language = mainChatScript.languages[1];
            }
            else if(player_ID == 1){
                language = mainChatScript.languages[0];
            }
            if(language == "Select a Language" || language == null){
                language = "English";
            }
            Debug.Log(language);
            newMessage.Content = prompt + " Translate the message from English to " + language + ".\n" + messageTemp;
        }
        
        messages.Add(newMessage);
        
        messageTemp = "";
        
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
            
            messageTemp = message.Content;
            Debug.Log(mainChatScript);
            mainChatScript.AddMessage(player_ID, messageTemp);
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

        messageTemp = res.Text;
        SendReply();
    }

    void UpdateClientEnvironment(){
        // Set HDR to not cloudy day if we are the client (aka not the server, player 1)(aka player 2)
        Debug.Log("Client environment updated!");
        RenderSettings.fogColor = futureFogcolor;
        RenderSettings.fogDensity = futureFogDensity;
        RenderSettings.skybox = futureSkybox;
        DynamicGI.UpdateEnvironment();
    }


    void SetClientPos(){
        futurePlayerSpawnPoint = GameObject.Find("Future Player Spawn Point");
        presentPlayerSpawnPoint = GameObject.Find("Present Player Spawn Point");

        Debug.Log("Future Player Spawn Point: " + futurePlayerSpawnPoint.transform);
        Debug.Log("Present Player Spawn Point: " + presentPlayerSpawnPoint.transform);
        Transform spawnPoint = IsServer ? presentPlayerSpawnPoint.transform : futurePlayerSpawnPoint.transform;

        xrOrigin = GameObject.Find("XR Origin (VR)").GetComponent<XROrigin>();

        xrOrigin.MoveCameraToWorldLocation(spawnPoint.position);
        xrOrigin.transform.rotation = spawnPoint.rotation;

        Debug.Log($"XR Origin moved to position: {xrOrigin.transform.position}, rotation: {xrOrigin.transform.rotation}");

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log($"Client Transform: {xrOrigin.transform.position}, rotation: {xrOrigin.transform.rotation}");


    }

[ServerRpc]
void RequestMoveServerRpc(Vector3 position, Quaternion rotation)
//This function and the client one below it handle moving the Network Player object for other clients, to make sure other clients see the other player model move too
{
    UpdatePlayerPositionClientRpc(position, rotation);
}

[ClientRpc]
void UpdatePlayerPositionClientRpc(Vector3 position, Quaternion rotation)
{
    if (!IsOwner)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}

    void Update()
    {
        if (IsOwner)
        {
            UpdateLocalTransforms();
            SyncPositionsServerRpc();
            if(mainChatScript != null){
                playerChat.text = mainChatScript.getText();
            }
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
        if(mainText == null && GameObject.FindWithTag("Chat") != null){
            IntializeMainText();
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