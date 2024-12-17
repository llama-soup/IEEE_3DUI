/// <summary>
/// Provides functionality for grabbable objects to work in multiplayer
/// When a player grabs an object, they gain ownership over the object, then the ownership returns to the server after the object is dropped
/// </summary>

using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedGrabbable : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private NetworkObject networkObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        grabInteractable = GetComponent<XRGrabInteractable>();
        networkObject = GetComponent<NetworkObject>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    // Request ownership over object when grabbed
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    // give ownership of object to client
    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        networkObject.ChangeOwnership(clientId);
    }
}