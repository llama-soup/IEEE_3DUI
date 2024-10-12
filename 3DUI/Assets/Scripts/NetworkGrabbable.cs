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

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        networkObject.ChangeOwnership(clientId);
    }
}