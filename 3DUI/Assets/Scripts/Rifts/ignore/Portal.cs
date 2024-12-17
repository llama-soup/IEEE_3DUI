using UnityEngine;
using UnityEngine.XR;

public class Portal : MonoBehaviour
{
    [Header("Portal Properties")]
    public Portal otherPortal;
    public Material portalMaterial;

    private Camera portalCamera;
    private RenderTexture leftEyeTexture;
    private RenderTexture rightEyeTexture;
    private Camera mainCamera;

    void Start()
    {
        GameObject camObj = new GameObject("Portal Camera");
        camObj.transform.parent = transform;
        
        portalCamera = camObj.AddComponent<Camera>();
        
        // Create render textures using main camera dimensions
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in scene");
            enabled = false;
            return;
        }

        leftEyeTexture = new RenderTexture(
            mainCamera.pixelWidth, 
            mainCamera.pixelHeight,
            24,
            RenderTextureFormat.DefaultHDR
        );
        leftEyeTexture.Create();
        
        rightEyeTexture = new RenderTexture(
            mainCamera.pixelWidth, 
            mainCamera.pixelHeight,
            24,
            RenderTextureFormat.DefaultHDR
        );
        rightEyeTexture.Create();
        
        // Setup portal camera
        portalCamera.enabled = true;
        portalCamera.depth = 0;
        portalCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        portalCamera.targetTexture = leftEyeTexture; // Initial assignment
        
        // Copy settings from main camera
        portalCamera.cullingMask = mainCamera.cullingMask;
        portalCamera.clearFlags = mainCamera.clearFlags;
        portalCamera.backgroundColor = mainCamera.backgroundColor;
        portalCamera.fieldOfView = mainCamera.fieldOfView;
        portalCamera.nearClipPlane = mainCamera.nearClipPlane;
        portalCamera.farClipPlane = mainCamera.farClipPlane;
    }

    void LateUpdate()
    {
        if (mainCamera == null || portalCamera == null || otherPortal == null)
            return;
                
        UpdatePortalCamera();

        // Render to both textures
        portalCamera.targetTexture = leftEyeTexture;
        portalCamera.Render();
        portalCamera.targetTexture = rightEyeTexture;
        portalCamera.Render();
        
        // Update the portal material with both textures
        if (otherPortal.portalMaterial != null)
        {
            otherPortal.portalMaterial.SetTexture("_LeftEyeTex", leftEyeTexture);
            otherPortal.portalMaterial.SetTexture("_RightEyeTex", rightEyeTexture);
        }
    }

    void UpdatePortalCamera()
    {
        Vector3 localPos = transform.InverseTransformPoint(mainCamera.transform.position);
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        
        Vector3 worldPos = otherPortal.transform.TransformPoint(localPos);
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * mainCamera.transform.rotation;
        Quaternion newRot = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;
        
        portalCamera.transform.SetPositionAndRotation(worldPos, newRot);
        portalCamera.projectionMatrix = mainCamera.projectionMatrix;
    }

    void OnDestroy()
    {
        if (leftEyeTexture != null)
        {
            leftEyeTexture.Release();
            DestroyImmediate(leftEyeTexture);
        }
        if (rightEyeTexture != null)
        {
            rightEyeTexture.Release();
            DestroyImmediate(rightEyeTexture);
        }
    }
}