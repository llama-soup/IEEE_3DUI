using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftCamera : MonoBehaviour
{
    public Camera riftCamera;
    public Transform rift1;
    public Transform rift2;
    public Camera playerCamera;
    
    private RenderTexture renderTexture;
    
    void Start()
    {
        // Create render texture for the rift
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        riftCamera.targetTexture = renderTexture;
        
        // Assign render texture to rift1's material
        rift1.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
    }

    void LateUpdate()
    {
        // Calculate camera position relative to rift2
        Vector3 playerFromRift1 = playerCamera.transform.position - rift1.position;
        riftCamera.transform.position = rift2.position + playerFromRift1;
        
        // Match rotation
        Quaternion rotationDiff = Quaternion.Inverse(rift1.rotation) * playerCamera.transform.rotation;
        riftCamera.transform.rotation = rift2.rotation * rotationDiff;
    }
}