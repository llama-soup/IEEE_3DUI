/*
Script used to spawn the main chat box once the main scene has been loaded.

Author: Jackson McDonald
*/
using Unity.Netcode;
using UnityEngine;

public class ChatSpawner : MonoBehaviour
{
    //Prefab of the main chat box
    [SerializeField] private GameObject mainChat; // Reference to the prefab

    //Spawns the main chat box as soon as the script is loaded in the scene
    void Start(){
        SpawnObject();
    }
    //Spawns the main chat box on the server
    public void SpawnObject()
    {
        if (NetworkManager.Singleton.IsServer) // Ensure this runs only on the server
        {
            // Instantiate the object on the server
            GameObject spawnedObject = Instantiate(mainChat);

            // Attach the NetworkObject component
            var networkObject = spawnedObject.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                // Spawn the object for all clients
                networkObject.Spawn();
            }
        }
    }
}

