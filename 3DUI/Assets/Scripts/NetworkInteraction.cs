using UnityEngine;
using Unity.Netcode;

public class NetworkInteraction : NetworkBehaviour
{
    public NetworkVariable<bool> PlayerActivated = new NetworkVariable<bool>();

    // Add any specific interaction logic here
    private void Update()
    {
        if (PlayerActivated.Value)
        {
            // Perform action when activated
            Debug.Log($"{gameObject.name} is activated!");
        }
        else
        {
            // Perform action when deactivated
            Debug.Log($"{gameObject.name} is deactivated!");
        }
    }
}