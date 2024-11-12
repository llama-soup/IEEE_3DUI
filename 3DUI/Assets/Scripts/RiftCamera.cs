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
        // Get the relative position of the player to the other rift
        Vector3 relativePos = otherRift.InverseTransformPoint(playerCamera.transform.position);
        
        // Convert to angles
        float verticalAngle = Mathf.Atan2(relativePos.y, relativePos.z) * Mathf.Rad2Deg;
        float horizontalAngle = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;
        
        // Apply the angles to this rift's camera, with additional 180 rotation around X axis to fix upside down view
        thisRiftCamera.transform.rotation = thisRift.rotation 
            * Quaternion.Euler(-verticalAngle, -horizontalAngle, 0) 
            * Quaternion.Euler(180, 90, 0);
    }
}