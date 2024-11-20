using UnityEngine;
using Unity.Netcode;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using System;
using TMPro;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    public Material presentSkybox;
    private Color presentFogColor = Color.blue;
    const  float presentFogDensity = 0.02f;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    private GameObject languageSetting;
    private GameObject microphones;
    public String language;
    public String microphone;
    public int player_ID;
    public TextMeshProUGUI messageDisplay;

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

    }

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

    void UpdateClientEnvironment(){
        // Set HDR to not cloudy day if we are the client (aka not the server, player 1)(aka player 2)
        RenderSettings.fogColor = presentFogColor;
        RenderSettings.fogDensity = presentFogDensity;
        RenderSettings.skybox = presentSkybox;
        DynamicGI.UpdateEnvironment();
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
        if (GameObject.FindWithTag("Language Options") != null)
        {
            languageSetting = GameObject.FindWithTag("Language Options");
            UpdateLanguage();
        }
        if (GameObject.FindWithTag("Microphones") != null)
        {
            microphones = GameObject.FindWithTag("Language Options");
            UpdateMicrophone();
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