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

    [Header("Debug")]
    public bool showDebugPlanes = false;
    public float debugPlaneSize = 1f;
    public Color portalPlaneColor = Color.blue;
    public Color clipPlaneColor = Color.red;

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
}