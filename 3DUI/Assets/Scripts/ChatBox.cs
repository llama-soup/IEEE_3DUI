using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ChatBox : MonoBehaviour
{
    [SerializeField] private TMP_Text textbox;
    private List<string> messages;
    // Start is called before the first frame update
    public string[] languages;
    void Start()
    {
        messages = new List<string>();
        languages = new string[3];
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
}
