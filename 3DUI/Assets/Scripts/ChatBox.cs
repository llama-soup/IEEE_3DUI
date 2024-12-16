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
    public string[] languages;
    void Start()
    {
        messages = new List<string>();
        languages = new string[2];
        messages.Add("This is the Chatbox. Player messages will appear here.");
        UpdateText();
        for(int i = 0; i < languages.Length; i++){
            languages[i] = "french";
        }
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
        MessageServerRpc(message);
        //messages.Add(message);
        //UpdateText();

    }
    public void Updatelanguage(int playerID, string language){
        //languages[playerID] = language;
        LanguageServerRpc(language, playerID);
    }
    public string getText(){
        return textbox.text;
    }
    [ClientRpc]
    private void LanguageClientRpc(string language, int id){
        languages[id] = language;
    }
    [ServerRpc]
    private void LanguageServerRpc(string language, int id){
        LanguageClientRpc(language, id);
    }
    [ClientRpc]
    private void MessageClientRpc(string newMessage){
        messages.Add(newMessage);
        UpdateText();
    }
    [ServerRpc]
    private void MessageServerRpc(string message){
        MessageClientRpc(message); 
    }
}
