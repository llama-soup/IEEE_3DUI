using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    private Renderer _renderer;
    private bool open = false;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is missing on the door object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("Controller entered the door trigger!");
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
