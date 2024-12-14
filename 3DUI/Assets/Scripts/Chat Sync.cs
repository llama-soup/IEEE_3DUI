using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SyncObject : NetworkBehaviour
{
    [SerializeField] private GameObject mainChat;
    private GameObject networkChat;

    private void Update()
    {
        if (IsServer)
        {
            // Update position on the server
            networkChat = mainChat;
        }
        else
        {
            // Apply synced position on clients
            mainChat = networkChat;
        }
    }
}

