/*
Script responsible for processing player inputs related to messaging the other player

Author: Jackson McDonald
*/
using UnityEngine;
using UnityEngine.InputSystem;

public class Translation : MonoBehaviour
{
    //The network player that sent the input
    private NetworkPlayer player;

    //Finds the player who will send the input
    private void Awake()
    {
        player = GetComponent<NetworkPlayer>();
    }
    //When the player presses the second button on the right controller it will trigger the function
    //Parameters:
    //  context is the state of the action that is used to trigger events in the function
    public void Translate(InputAction.CallbackContext context)
    {
        //Starts recording when button is pressed
        if (context.started && player != null)
        {
            //Triggers function in the network player to start the recording
            player.SpaceRecord();
        }
        //Ends recording when button is released
        else if(context.canceled && player != null)
        {
            //Triggers funtion in network player to end recording
            player.EndRecord();
        }
    }
}
