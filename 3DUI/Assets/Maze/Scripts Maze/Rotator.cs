using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    private Renderer _renderer;
    

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is missing on the door object!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("Controller entered the pickup trigger!");
            this.gameObject.SetActive(false);

    }

    
}
