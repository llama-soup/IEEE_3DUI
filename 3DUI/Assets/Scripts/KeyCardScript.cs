using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class KeyCardScript : MonoBehaviour
{
    public Component OverlapBox;
    public AudioSource audioSource;
    private Boolean isActivatedAlready = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Keycard" && isActivatedAlready == false){
            OpenDoor();
        } 
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    [ServerRpc]
    void OpenDoor(){
        isActivatedAlready = true;
        audioSource.PlayOneShot(audioSource.clip);
        Debug.Log("Opening Door, placeholder. Will fill in with something");
    }
}
