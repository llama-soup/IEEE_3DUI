/// <summary>
/// Manages host and join UI screens in lobby scene
/// </summary>

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText;
    public TMP_InputField joinCodeInputField;
    public Button hostButton;
    public Button joinButton;
    public LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager.OnJoinCodeGenerated += UpdateJoinCodeDisplay;
        
        // Add listeners to buttons
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void UpdateJoinCodeDisplay(string joinCode)
    {
        joinCodeText.text = $"Join Code: {joinCode}";
    }

    private void OnHostButtonClicked()
    {
        lobbyManager.HostGame();
        hostButton.interactable = false;
    }

    private void OnJoinButtonClicked()
    {
        string inputJoinCode = joinCodeInputField.text;
        Debug.Log("Input Code is: " + inputJoinCode);
        if (!string.IsNullOrEmpty(inputJoinCode))
        {
            lobbyManager.JoinGame(inputJoinCode);
        }
        else
        {
            Debug.LogWarning("Join code is empty. Please enter a valid join code.");
        }
    }

    private void OnDestroy()
    {
        lobbyManager.OnJoinCodeGenerated -= UpdateJoinCodeDisplay;
    }
}