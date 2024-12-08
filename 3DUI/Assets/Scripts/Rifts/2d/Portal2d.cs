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
    private readonly Vector3 leftEyeOffset = new Vector3(0.4f, 0, 0);
    private readonly Vector3 rightEyeOffset = new Vector3(-0.4f, 0, 0);

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
        
        // Create and setup render textures
        leftRenderTexture = new RenderTexture(Screen.width/2, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        rightRenderTexture = new RenderTexture(Screen.width/2, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        
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
        cam.fieldOfView = 90f;
        return cam;
    }

    void LateUpdate()
    {
        // leftEyeCamera.fieldOfView = playerCamera.fieldOfView;
        // rightEyeCamera.fieldOfView = playerCamera.fieldOfView;
        // Calculate parent position and rotation
        Vector3 playerOffsetFromPortal = transform.InverseTransformPoint(playerCamera.transform.position);
        playerOffsetFromPortal = new Vector3(-playerOffsetFromPortal.x, playerOffsetFromPortal.y, -playerOffsetFromPortal.z);
        Vector3 newPosition = otherPortal.transform.TransformPoint(playerOffsetFromPortal);

        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * playerCamera.transform.rotation;
        Quaternion newRotation = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;

        // Update parent transform (cameras will follow)
        portalCameraParent.SetPositionAndRotation(newPosition, newRotation);
    }

    void OnDestroy()
    {
        if (leftRenderTexture != null)
            leftRenderTexture.Release();
        if (rightRenderTexture != null)
            rightRenderTexture.Release();
    }
}