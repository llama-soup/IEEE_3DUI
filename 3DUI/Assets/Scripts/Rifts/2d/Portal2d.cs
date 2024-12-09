using UnityEngine;
using UnityEngine.XR;

public class Portal2d : MonoBehaviour
{
    [Header("Portal Properties")]
    public Portal2d otherPortal;
    public Material portalMaterial;

    private Camera playerCamera;
    private Transform portalCameraParent;
    private Camera leftEyeCamera;
    private Camera rightEyeCamera;
    private RenderTexture leftRenderTexture;
    private RenderTexture rightRenderTexture;
    
    // Standard IPD (Interpupillary Distance)
    private readonly Vector3 leftEyeOffset = new Vector3(0.032f, 0, 0);
    private readonly Vector3 rightEyeOffset = new Vector3(-0.032f, 0, 0);

    void Start()
    {

        playerCamera = Camera.main;
        
        // Create parent object for portal cameras
        GameObject parentObj = new GameObject("PortalCameraParent");
        portalCameraParent = parentObj.transform;
        portalCameraParent.parent = transform;
        
        // Setup left eye camera
        leftEyeCamera = CreateEyeCamera("LeftEyeCamera", leftEyeOffset);
        rightEyeCamera = CreateEyeCamera("RightEyeCamera", rightEyeOffset);

        leftEyeCamera.transform.localRotation = Quaternion.Euler(0, -8f, 0);
        rightEyeCamera.transform.localRotation = Quaternion.Euler(0, 8f, 0);
        
        // Create and setup render textures
        leftRenderTexture = new RenderTexture(Screen.width, Screen.height * 2, 72, RenderTextureFormat.DefaultHDR);
        rightRenderTexture = new RenderTexture(Screen.width, Screen.height * 2, 72, RenderTextureFormat.DefaultHDR);
        
        leftEyeCamera.targetTexture = leftRenderTexture;
        rightEyeCamera.targetTexture = rightRenderTexture;
        
        // Assign textures to portal material
        otherPortal.portalMaterial.SetTexture("_LeftEyeTex", leftRenderTexture);
        otherPortal.portalMaterial.SetTexture("_RightEyeTex", rightRenderTexture);
    }

    private Camera CreateEyeCamera(string name, Vector3 offset)
    {
        GameObject eyeCam = new GameObject(name);
        eyeCam.transform.parent = portalCameraParent;
        eyeCam.transform.localPosition = offset;
        
        Camera cam = eyeCam.AddComponent<Camera>();
        cam.enabled = true;
        cam.fieldOfView = 88f;
        return cam;
    }

    private void SetNearClipPlane(Camera portalEyeCamera, bool isLeftEye) 
    {
        Transform clipPlane = transform;
        Camera playerEyeCamera = isLeftEye ? leftEyeCamera : rightEyeCamera;
        
        // Calculate dot product to determine which side of the portal we're on
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, 
            transform.position - portalEyeCamera.transform.position));

        // Convert portal plane to camera space
        Vector3 camSpacePos = portalEyeCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalEyeCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);

        // Skip oblique projection if too close to prevent artifacts
        if (Mathf.Abs(camSpaceDst) > 0.3f) 
        {
            Vector4 clipPlaneCameraSpace = new Vector4(
                camSpaceNormal.x, 
                camSpaceNormal.y, 
                camSpaceNormal.z, 
                camSpaceDst
            );

            // Calculate oblique projection matrix for specific eye
            portalEyeCamera.projectionMatrix = playerEyeCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        } 
        else 
        {
            // Reset to original eye-specific projection
            portalEyeCamera.projectionMatrix = playerEyeCamera.projectionMatrix;
        }
    }

    void LateUpdate()
    {
        // Calculate parent position and rotation
        Vector3 playerOffsetFromPortal = transform.InverseTransformPoint(playerCamera.transform.position);
        playerOffsetFromPortal = new Vector3(-playerOffsetFromPortal.x, playerOffsetFromPortal.y, -playerOffsetFromPortal.z);
        Vector3 newPosition = otherPortal.transform.TransformPoint(playerOffsetFromPortal);

        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * playerCamera.transform.rotation;
        Quaternion newRotation = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;

        // Update parent transform (cameras will follow)
        portalCameraParent.SetPositionAndRotation(newPosition, newRotation);

        // Apply oblique projection to both eye cameras
        float distanceToPortal = Vector3.Distance(playerCamera.transform.position, transform.position);
        SetNearClipPlane(leftEyeCamera, true);
        SetNearClipPlane(rightEyeCamera, false);
    }

    void OnDestroy()
    {
        if (leftRenderTexture != null)
            leftRenderTexture.Release();
        if (rightRenderTexture != null)
            rightRenderTexture.Release();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            // Get the direction from portal to object
            Vector3 portalToObject = other.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, portalToObject);

            // Only teleport if object is entering from the front
            if (dotProduct < 0f)
            {
                // Calculate position relative to this portal
                Vector3 objectOffsetFromPortal = transform.InverseTransformPoint(other.transform.position);
                
                // Flip the x and z coordinates for proper exit orientation
                objectOffsetFromPortal = new Vector3(-objectOffsetFromPortal.x, 
                                                objectOffsetFromPortal.y, 
                                                -objectOffsetFromPortal.z);
                
                // Calculate the new position relative to other portal
                Vector3 newPosition = otherPortal.transform.TransformPoint(objectOffsetFromPortal);
                
                // Calculate rotation
                Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * other.transform.rotation;
                Quaternion newRotation = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;
                
                // If object has rigidbody, preserve velocity through portal
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);
                    relativeVelocity = new Vector3(-relativeVelocity.x, relativeVelocity.y, -relativeVelocity.z);
                    Vector3 newVelocity = otherPortal.transform.TransformDirection(relativeVelocity);
                    
                    // Teleport and set new velocity
                    rb.position = newPosition;
                    rb.rotation = newRotation;
                    rb.velocity = newVelocity;
                }
                else
                {
                    // Teleport non-rigidbody object
                    other.transform.SetPositionAndRotation(newPosition, newRotation);
                }
            }
        }
    }
}