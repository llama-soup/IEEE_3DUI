using Unity.Netcode;
using UnityEngine;

public class ChatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mainChat; // Reference to the prefab

    void Start(){
        SpawnObject();
    }

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

