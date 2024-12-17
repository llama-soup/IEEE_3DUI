/*
This script contains all of the functions for the global chatbox prefab. 
This prefab serves as an intermidiate between players and allows for 
the transfer of players' language settings and messages.

Author: Jackson McDonald
*/
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ChatBox : NetworkBehaviour
{
    //Textbox that stores the text of the chatbox
    [SerializeField] private TMP_Text textbox;
    //List of the four most recent messages sent
    private List<string> messages;
    //Array of player language settings 
    //The index used for each player is equal to their player ID
    public string[] languages;
    //Initializes the variables and sets up initial text to be displayed
    void Start()
    {
        messages = new List<string>();
        languages = new string[2];
        messages.Add("This is the Chatbox. Player messages will appear here.");
        UpdateText();
    }
    //Updates the text in the chatbox to match the most recent messages
    private void UpdateText(){
        string displayedText = "";
        foreach(string message in messages){
            displayedText += message +"\n";
        }
        textbox.text = displayedText;
    }
    //Adds a message sent by the player to the list of messages
    //Parameters:
    //  playerID is the player ID of the player who sent the message
    //  message is the contents of the message to be added to the list
    public void AddMessage(int playerID, string message){
        string num = playerID.ToString();
        message = "player" +  num + ": " + message;
        //Asks client to update messages for all clients
        MessageServerRpc(message);
    }
    //Updates the language setting stored for the player in the array
    //Parameters:
    //  playerID is the player ID of the player who updated their language setting
    //  language is the language that the player is updating their settings to
    public void Updatelanguage(int playerID, string language){
        //Requests for the server to update the language for all clients
        LanguageServerRpc(language, playerID);
    }
    //Returns the text in the chat box to be displayed
    public string getText(){
        return textbox.text;
    }
    //Updates the language array for all clients with the new language selected.
    //Parameters:
    //  id is the player ID of the player who updated their language setting
    //  language is the language that the player is updating their settings to
    [ClientRpc]
    private void LanguageClientRpc(string language, int id){
        languages[id] = language;
    }
    //Calls for the language settings to be updated for all clients
    //Parameters:
    //  id is the player ID of the player who updated their language setting
    //  language is the language that the player is updating their settings to
    [ServerRpc]
    private void LanguageServerRpc(string language, int id){
        LanguageClientRpc(language, id);
    }
    //Adds the new Message to the list of messages for all clients
    //Parameters:
    //  newMessage is the player message to be added to the chat box
    [ClientRpc]
    private void MessageClientRpc(string newMessage){
        if(messages.Count >= 4){
            messages.RemoveAt(0);
        }
        messages.Add(newMessage);
        UpdateText();
    }
    //Calls for new message to be added to the list of messages for all clients
    //Parameters:
    //  newMessage is the player message to be added to the chat box
    [ServerRpc]
    private void MessageServerRpc(string message){
        MessageClientRpc(message); 
    }
}
