using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbable : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private NetworkObject networkObject;
    private Rigidbody rb;

    private bool isBeingHeld = false;
    private Transform attachTransform;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        grabInteractable = GetComponent<XRGrabInteractable>();
        networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (IsServer) return;
        RequestGrabServerRpc();
        attachTransform = args.interactorObject.transform;
        isBeingHeld = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (IsServer) return;
        RequestReleaseServerRpc(rb.velocity, rb.angularVelocity);
        isBeingHeld = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestGrabServerRpc(ServerRpcParams rpcParams = default)
    {
        networkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        UpdateGrabbedStateClientRpc(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestReleaseServerRpc(Vector3 velocity, Vector3 angularVelocity, ServerRpcParams rpcParams = default)
    {
        networkObject.RemoveOwnership();
        UpdateReleasedStateClientRpc(velocity, angularVelocity);
    }

    [ClientRpc]
    private void UpdateGrabbedStateClientRpc(bool isGrabbed)
    {
        if (rb != null)
        {
            rb.isKinematic = isGrabbed;
            rb.useGravity = !isGrabbed;
        }
    }

    [ClientRpc]
    private void UpdateReleasedStateClientRpc(Vector3 velocity, Vector3 angularVelocity)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (isBeingHeld && IsOwner)
        {
            Vector3 targetPosition = attachTransform.position;
            Vector3 targetRotation = attachTransform.rotation.eulerAngles;

            rb.MovePosition(targetPosition);
            rb.MoveRotation(Quaternion.Euler(targetRotation));
        }
    }
}