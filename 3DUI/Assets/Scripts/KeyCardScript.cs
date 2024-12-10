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

    public GameObject doorToOpen;
    private Vector3 doorOpenLoc = new Vector3(-22.3f, -1.82f, 15.6f);
    private Vector3 doorOpenRot = new Vector3(0f, -165f, 0f);

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
        doorToOpen.transform.position = doorOpenLoc;
        doorToOpen.transform.rotation = Quaternion.Euler(doorOpenRot);
    }
}
