

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    private Renderer _renderer;
    private bool open = false;
    public NetworkPlayer netPlayer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is missing on the door object!");
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //         Debug.Log("Controller entered the door trigger!");
    //         if(other.CompareTag("PlayerTag") && this.gameObject.CompareTag("EnterMaze"))
    //         {
    //             Debug.Log("Entered the maze");

    //             netPlayer = other;
    //             netPlayer.InitializeMazeText();

    //                 // Initialize the maze text elements


    //             // Set initial text values
    //             countText.text = "Count: 0";
    //             timerText.text = "Time: 00:00";
    //             environmentalFactsText.text = "TEST IF HERE";
    //         }
    //         ToggleDoor();
    // }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Controller entered the door trigger!");

        // Check if the colliding object has the correct tag and this door is tagged "EnterMaze"
        if (other.CompareTag("PlayerTag") && this.gameObject.CompareTag("EnterMaze"))
        {
            Debug.Log("Entered the maze");

            // Try to get the NetworkPlayer component from the colliding object
            netPlayer = other.GetComponent<NetworkPlayer>();

            if (netPlayer != null)
            {
                // Call the InitializeMazeText method on the NetworkPlayer
                netPlayer.InitializeMazeText();

                // Set initial text values (ensure these are properly set up in the NetworkPlayer script)
                netPlayer.countText.text = "Count: 0";
                netPlayer.timerText.text = "Time: 00:00";
                netPlayer.environmentalFactsText.text = "TEST IF HERE";
            }
            else
            {
                Debug.LogError("The colliding object does not have a NetworkPlayer component!");
            }
        }

        ToggleDoor();
    }

    public void ToggleDoor()
    {
        Debug.Log("ToggleDoor called!");

        if (_renderer == null)
        {
            Debug.LogError("Renderer is missing on the door object!");
            return;
        }

        if (open)
        {
            _renderer.transform.Rotate(Vector3.up, 90);
            _renderer.transform.position += new Vector3(
                0.5f * _renderer.bounds.size.z, 
                0, 
                5 * _renderer.bounds.size.x
            );
        }
        else
        {
            _renderer.transform.Rotate(Vector3.up, -90);
            _renderer.transform.position += new Vector3(
                -0.5f * _renderer.bounds.size.x, 
                0, 
                -5 * _renderer.bounds.size.z
            );
        }

        open = !open;
        Debug.Log("Door toggled. Is Open: " + open);
    }
}
