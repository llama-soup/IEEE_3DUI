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
    private List<NetworkVariable<string>> netMessage;
    private NetworkVariable<string>[] netLanguage;
    void Start()
    {
        messages = new List<string>();
        languages = new string[2];
        netMessage = new List<NetworkVariable<string>>();
        netLanguage = new NetworkVariable<string>[2];
        messages.Add("This is the Chatbox. Player messages will appear here.");
        UpdateText();
        ChangeNetLanguage();
        ChangeNetMessage();
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
        ChangeNetMessage();
        UpdateText();
    }

    public void Updatelanguage(int playerID, string language){
        languages[playerID] = language;
        ChangeNetLanguage();
    }
    public string getText(){
        return textbox.text;
    }
    private void ChangeNetMessage(){
        for(int i = 0; i < messages.Count; i++){
            netMessage[i].Value = messages[i];
        }
    }
    private void ChangeNetLanguage(){
        for(int i = 0; i < languages.Length; i++){
            netLanguage[i].Value = languages[i];
        }
    }
    void Update(){
        if(netMessage.Count > messages.Count){
            messages.Add(netMessage[netMessage.Count-1].Value);
            UpdateText();
        }
        else{
            if(netMessage[0].Value != messages[0]){
                for(int i = 0; i < netMessage.Count; i++){
                    messages[i] = netMessage[i].Value;
                }
                UpdateText();                
            }
        }
        if(netLanguage[0].Value != languages[0] || netLanguage[1].Value != languages[1]){
            for(int i = 0; i < netLanguage.Length; i++){
                languages[i] = netLanguage[i].Value;
            }
        }
    }
}
