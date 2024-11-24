using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

public class VRPortal : MonoBehaviour
{
    [Header("Portal Properties")]
    public VRPortal otherPortal;
    public Material leftEyePortalMaterial;
    public Material rightEyePortalMaterial;

    private Camera leftPortalCamera;
    private Camera rightPortalCamera;
    private Camera xrCamera;
    private RenderTexture leftRenderTexture;
    private RenderTexture rightRenderTexture;
    private XROrigin xrOrigin;

    void Start()
    {
        // Get XR components
        xrOrigin = FindObjectOfType<XROrigin>();
        xrCamera = xrOrigin.Camera;
        
        // Setup left eye camera
        leftPortalCamera = transform.Find("LeftPortalCamera").GetComponent<Camera>();
        leftRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        leftPortalCamera.targetTexture = leftRenderTexture;
        otherPortal.leftEyePortalMaterial.mainTexture = leftRenderTexture;
        
        // Setup right eye camera
        rightPortalCamera = transform.Find("RightPortalCamera").GetComponent<Camera>();
        rightRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        rightPortalCamera.targetTexture = rightRenderTexture;
        otherPortal.rightEyePortalMaterial.mainTexture = rightRenderTexture;
        
        // Enable cameras
        leftPortalCamera.enabled = true;
        rightPortalCamera.enabled = true;
    }

    void LateUpdate()
    {
        UpdatePortalCamera(rightPortalCamera, Camera.StereoscopicEye.Right);
        UpdatePortalCamera(leftPortalCamera, Camera.StereoscopicEye.Left);
    }

    private void UpdatePortalCamera(Camera portalCamera, Camera.StereoscopicEye eye)
    {
        // Get eye position from XR camera
        Vector3 eyePos = xrCamera.GetStereoViewMatrix(eye).inverse.GetColumn(3);
        
        // Calculate portal offset while preserving eye separation
        Vector3 offsetFromPortal = transform.InverseTransformPoint(eyePos);
        // For left eye, preserve the negative x offset
        if (eye == Camera.StereoscopicEye.Left)
        {
            offsetFromPortal = new Vector3(offsetFromPortal.x, offsetFromPortal.y, -offsetFromPortal.z);
        }
        else
        {
            offsetFromPortal = new Vector3(-offsetFromPortal.x, offsetFromPortal.y, -offsetFromPortal.z);
        }
        Vector3 newCameraPosition = otherPortal.transform.TransformPoint(offsetFromPortal);
        
        // Rest of the method remains the same
        Quaternion eyeRot = xrCamera.transform.rotation;
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * eyeRot;
        Quaternion newRotation = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;
        
        portalCamera.transform.SetPositionAndRotation(newCameraPosition, newRotation);
        portalCamera.projectionMatrix = xrCamera.GetStereoProjectionMatrix(eye);
    }

    void OnDestroy()
    {
        if (leftRenderTexture != null) leftRenderTexture.Release();
        if (rightRenderTexture != null) rightRenderTexture.Release();
    }
}