using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorXRInteract : MonoBehaviour
{
    private Renderer _renderer;
    private bool open = false;

    void Start()
    {
        Debug.Log("STARTED");
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is missing on the door object!");
        }
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
        }
        else
        {
            _renderer.transform.Rotate(Vector3.up, -90);
        }

        open = !open;
        Debug.Log("Door toggled. Is Open: " + open);
    }

    // Trigger the door toggle when the controller enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller")) // Ensure the controller has the "Controller" tag
        {
            Debug.Log("Controller entered the door trigger!");
            ToggleDoor();
        }
    }
}
