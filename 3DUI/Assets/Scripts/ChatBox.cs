using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ChatBox : NetworkBehaviour
{
    [SerializeField] private TMP_Text textbox;
    private List<string> messages;
    // Start is called before the first frame update
    public string[] languages;
    private TMP_Text networkChat;
    private string[] networkLanguages;
    void Start()
    {
        messages = new List<string>();
        languages = new string[2];
        messages.Add("This is the Chatbox. Player messages will appear here.");
        UpdateText();
    }

    private void UpdateText(){
        string displayedText = "";
        foreach(string message in messages){
            displayedText += message +"\n";
        }
        textbox.text = displayedText;
    }

    public void AddMessage(int playerID, string message){
        if(messages.Count >= 4){
            messages.RemoveAt(0);
        }
        string num = playerID.ToString();
        message = "player" +  num + ": " + message;
        messages.Add(message);
        UpdateText();
    }

    public void Updatelanguage(int playerID, string language){
        languages[playerID] = language;
    }
    public string getText(){
        return textbox.text;
    }

    private void Update()
    {
        if (IsServer)
        {
            // Update position on the server
            networkLanguages = languages;
            networkChat = textbox;
        }
        else
        {
            // Apply synced position on clients
            languages = networkLanguages;
            textbox = networkChat;
        }
    }
}
