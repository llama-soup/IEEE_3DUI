

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

         if (netPlayer == null)
        {
            netPlayer = FindObjectOfType<NetworkPlayer>();
            if (netPlayer == null)
            {
                Debug.LogError("NetworkPlayer script not found in the scene!");
            }
        }
        Debug.Log(netPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Controller entered the door trigger!");

        // Check if the collided object has the tag "EnterMaze"
        // if (other.CompareTag("EnterMaze"))
        // {
            Debug.Log("Object with tag 'EnterMaze' collided.");

            // Call ShowMazeText on the NetworkPlayer script
            if (netPlayer != null)
            {
                netPlayer.ShowMazeText();
                Debug.Log("ShowMazeText function called on NetworkPlayer.");
            }
            else
            {
                Debug.LogError("NetworkPlayer reference is null. Assign it in the inspector.");
            }
        //}

        // Toggle the door
        ToggleDoor();
    }


    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Controller entered the door trigger!");

    //     ToggleDoor();
    // }

    // void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("Controller entered the door trigger!");
    //     if (collision.gameObject.CompareTag("EnterMaze"))
    //     {
    //         Debug.Log("testing this");
    //     }
    //     ToggleDoor();
    // }


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
