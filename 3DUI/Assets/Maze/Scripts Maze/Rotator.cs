//Class to handle operations with the pickup objects 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    private Renderer _renderer;
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
    // Update is called once per frame
    void Update()
    {
        //Rotation of the pickup prefabs
        transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
    }


    public void OnTriggerEnter(Collider other)
    {
            Debug.Log("Controller entered the pickup trigger!");
            this.gameObject.SetActive(false);
            netPlayer.newspaperCount = netPlayer.newspaperCount+1;
            netPlayer.SetCountText();
            netPlayer.setEnvironmentalFact();
            if (netPlayer.newspaperCount == 11)
            {
                netPlayer.timerStopped = true; // Stop the timer
                //StartCoroutine(ShowCongratsMessageWithDelay());
            }

    }

    
}
