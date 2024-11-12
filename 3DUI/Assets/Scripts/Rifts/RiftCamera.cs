using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftCamera : MonoBehaviour
{
    public Camera thisRiftCamera;        // The camera attached to this rift
    public Camera otherRiftCamera;
    public Transform thisRift;       // The rift this camera is attached to
    public Transform otherRift;      // The rift the player looks through
    public Camera playerCamera;      // The main player camera
    
    private RenderTexture renderTexture;
    public float x;
    public float y;
    public float z;
    
    void Start()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        thisRiftCamera.targetTexture = renderTexture;
        thisRift.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
        
        // Fix camera position to this rift
        otherRiftCamera.transform.position = thisRift.position;
    }

    void LateUpdate()
    {
        // Get direction from other rift to player
        Vector3 directionToPlayer = (playerCamera.transform.position - otherRift.position).normalized;
        
        // Project the direction onto the horizontal and vertical planes
        Vector3 horizontalDir = Vector3.ProjectOnPlane(directionToPlayer, Vector3.up).normalized;
        Vector3 verticalDir = Vector3.ProjectOnPlane(directionToPlayer, Vector3.right).normalized;
        
        // Calculate angles
        float horizontalAngle = Vector3.SignedAngle(otherRift.forward, horizontalDir, Vector3.up);
        float verticalAngle = Vector3.SignedAngle(otherRift.forward, verticalDir, Vector3.right);
        
        // Apply rotation to this rift's camera
        otherRiftCamera.transform.rotation = otherRift.rotation 
            * Quaternion.Euler(verticalAngle, horizontalAngle, 0) 
            * Quaternion.Euler(180, 0, 0);
    }
}