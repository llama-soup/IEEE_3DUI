/*
The prefab for each player that is spawned by the network.
Contains the instructions for the players skyboxes, controls, communication, and UI

Authors: Tom Roff, Jackson McDonald, and Emre Guvenilir
*/
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
    public XROrigin xrOrigin;
    public Renderer[] meshToDisable;
    //Start of traslation variables
    //Language menu
    private GameObject languageSetting;
    //Microphone menu
    private GameObject microphones;
    //The players chosen language setting
    private string language;
    //The language setting for the other player
    private string otherLanuage;
    //The microphone selected for recording
    private string microphone;
    //The players ID based on the number of people in the lobby when the player spawned
    public int player_ID;
    //Temperary variable to store the string from voice recording before translation
    private string messageTemp;
    //List of the messages to send to the Open AI for translation
    private List<ChatMessage> messages = new List<ChatMessage>();
    //Prompt for the AI translator
    private string prompt = "Act as a translator for the user. Your response should only include the translation requested. Don't break character. Don't ever mention that you are an AI model.";
    //Output file name for the player's recorded message
    private readonly string fileName = "output.wav";
    //maximum recording duration in seconds
    private readonly int duration = 10;
    //The audio clip of the recorded message
    private AudioClip clip;
    //Boolean to see if there is a recording currently in progress
    private bool isRecording;
    //Boolean used to end recording and send the message to translation
    private bool stop;
    //How long the recording has been happening for
    private float time;
    //The Open AI environment being used
    private OpenAIApi openai = new OpenAIApi();
    //The main chat box that contains the player messages and language settings
    private GameObject mainText;
    //The script for the main chat box
    private ChatBox mainChatScript;
    //The prefab for the canvas that displays all messages to the user
    public GameObject canvasPrefab;
    //The canvas prefab that is spawned by the network player
    private GameObject playerCanvas;
    //The text box on the player canvas used to display messages
    private TMP_Text playerChat;
    //end of translate variables
    public GameObject mazeTextPrefab; // Prefab containing the text elements
    private GameObject mazeTextPriv; // Instance of the prefab
    public TMP_Text countText; // For CountText
    public TMP_Text timerText; // For Timer
    public TMP_Text environmentalFactsText; // For EnvironmentalFacts
    public Button restartButton; // For Restart

    public int newspaperCount;
    public bool timerStopped = true;
    [SerializeField] float remainingTime = 120;
    public string[] EnvironmentalFacts = {
        "Recycling one ton of paper saves 17 trees.",
        "Plastic takes up to 1000 years to decompose.",
        "Oceans produce at least 50% of the Earth’s oxygen.",
        "Deforestation contributes about 12% to 20% of global greenhouse gas emissions.",
        "Around 1 million species are at risk of extinction due to climate change.",
        "Another fact here",
        "Yet Another",
        "Anotha one",
        "Yes",
        "Climate Change is bad",
        "Another"
    };
    //end of maze text vars
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
            //Establishes the player's ID
            player_ID = NetworkManager.Singleton.ConnectedClients.Count;
            //Sets default language to english
            language = "english";
            //Establishes boolean that stops the audio recording.
            stop = false;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!IsServer){
            UpdateClientEnvironment();
            SetClientPos();
        }
        InitializeMazeText();

    }
    //Initializes the canvas prefab that is used to display all of the text to the player
    private void IntializeMainText(){
        mainText = GameObject.FindWithTag("Chat");
        mainChatScript = mainText.GetComponent<ChatBox>();
        mainChatScript.Updatelanguage(player_ID, language);

        playerCanvas = Instantiate(canvasPrefab, transform);
        playerCanvas.transform.localPosition = new Vector3(0, 1.5f, 2.0f); // Adjust position relative to the player
        playerChat = playerCanvas.GetComponentInChildren<TMP_Text>();
    }
    
    //Updates the language to what the player selects in the menu
    void UpdateLanguage(){
        if (languageSetting.TryGetComponent(out TMP_Dropdown component))
        {
            int pickedIndex = component.value;
            language = component.options[pickedIndex].text;
            //Updates the language setting in the array stored by the main chat box
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
            //records the audio clip
            clip = Microphone.Start(microphone, false, duration, 44100);
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
        //Creates the chat messages needed for translation
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = messageTemp
        };

        //Makes the prompt
        if (messages.Count == 0) 
        {
            if(player_ID == 0){
                otherLanuage = mainChatScript.languages[1];
            }
            else if(player_ID == 1){
                otherLanuage = mainChatScript.languages[0];
            }
            if(otherLanuage == "Select a Language" || otherLanuage == null){
                otherLanuage = "English";
            }
            Debug.Log(otherLanuage);
            newMessage.Content = prompt + " Translate the message from English to " + otherLanuage + ".\n" + messageTemp;
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
            //Sends the newly translated message to the main chat box to be seen by all players
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
        //Ends the recording
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
        //Stores the text version of the recording in the temperary variable for the translator.
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
    //Updates the network player every frame
    void Update()
    {
        if (IsOwner)
        {
            UpdateLocalTransforms();
            SyncPositionsServerRpc();
            //Sets the text on the players display canvas equal to that on the global chat box
            if(mainChatScript != null && playerChat != null){
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
        //Checks the main chat box has been spawned yet only if the chat box hasn't been initialized
        if((mainText == null || playerChat == null) && GameObject.FindWithTag("Chat") != null){
            IntializeMainText();
        }
        //Process for recording the messages
        if (isRecording)
        {
            //Keeps track of how long the recording has been
            time += Time.deltaTime;
            //Stops the recording if button released or max duration has been hit
            if (time >= duration || stop)
            {
                time = 0;
                isRecording = false;
                stop = false;
                EndRecording();
            }
        }

        if(!timerStopped)
        {
                if(remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else if(remainingTime<0)
            {
                remainingTime = 0;
                environmentalFactsText.text = "You failed! Click to restart";
                timerStopped = true;
            }
            int minutes = Mathf.FloorToInt(remainingTime/60);
            int seconds = Mathf.FloorToInt(remainingTime%60);
            countText.text = string.Format("{0:00}:{1:00}",minutes,seconds); 
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

    public void InitializeMazeText()
    {
        // Instantiate the prefab
        mazeTextPriv = Instantiate(mazeTextPrefab, transform);
        mazeTextPriv.transform.localPosition = new Vector3(0, 1.5f, 2.0f);

        // Get references to the UI components
        countText = mazeTextPriv.transform.Find("CountText").GetComponent<TMP_Text>();
        timerText = mazeTextPriv.transform.Find("Timer").GetComponent<TMP_Text>();
        environmentalFactsText = mazeTextPriv.transform.Find("EnvironmentalFacts").GetComponent<TMP_Text>();
        
    }

    public void ShowMazeText()
    {
        Debug.Log("SHOWING");
        // countText.alpha = 1f;
        // timerText.alpha = 1f;
        // environmentalFactsText.alpha = 1f;
        //InitializeMazeText();
        timerStopped = false; // Start the timer
    }

    public void SetCountText() 
    {
        countText.text =  "Newspaper Count: " + newspaperCount.ToString() + "/11";
    }

    public void setEnvironmentalFact()
    {
        environmentalFactsText.text = EnvironmentalFacts[newspaperCount-1];
    }

    private void OnTriggerEnter(Collider other)
    {
        //ShowMazeText();
        Debug.Log("On trigger enter actually works");
        if (other.CompareTag("EnterMaze"))
        {
            Debug.Log("Entered the maze");

            ShowMazeText();
        }

        if (other.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            newspaperCount = newspaperCount + 1;
            SetCountText();
            setEnvironmentalFact();

            if (newspaperCount == 11)
            {
                timerStopped = true; // Stop the timer
                //StartCoroutine(ShowCongratsMessageWithDelay());
            }
        }
    }

//     private IEnumerator ShowCongratsMessageWithDelay()
//    {
//       yield return new WaitForSeconds(5); // Wait for 5 seconds
//       envFacts.text = "Congratulations! You've collected all the environmental stories!";
//    }

}