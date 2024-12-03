using UnityEngine;
using UnityEngine.InputSystem;

public class Translation : MonoBehaviour
{
    private NetworkPlayer player;
    // Start is called before the first frame update
    //Finds the player
    private void Awake()
    {
        player = GetComponent<NetworkPlayer>();
    }
    //When the player presses the second button on the right controller it will trigger the function
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
