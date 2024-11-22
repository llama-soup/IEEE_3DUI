using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Properties")]
    public Portal otherPortal;
    public Material portalMaterial;  // This should be the material on THIS portal's mesh renderer

    private Camera portalCamera;
    private Camera playerCamera;
    private RenderTexture renderTexture;

    void Start()
    {
        playerCamera = Camera.main;
        portalCamera = GetComponentInChildren<Camera>();
        
        // Create and setup render texture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        portalCamera.targetTexture = renderTexture;
        
        // Assign render texture to OTHER portal's material
        otherPortal.portalMaterial.mainTexture = renderTexture;
        
        // Setup portal camera
        portalCamera.enabled = true;
        portalCamera.fieldOfView = playerCamera.fieldOfView;
    }

    void LateUpdate()
    {
        // Get player's offset from portal in local space
        Vector3 playerOffsetFromPortal = transform.InverseTransformPoint(playerCamera.transform.position);
        
        // Flip the x and z coordinates to mirror the position correctly
        playerOffsetFromPortal = new Vector3(-playerOffsetFromPortal.x, playerOffsetFromPortal.y, -playerOffsetFromPortal.z);
        
        // Transform the mirrored position to world space using the other portal
        Vector3 newCameraPosition = otherPortal.transform.TransformPoint(playerOffsetFromPortal);

        // Calculate rotation
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * playerCamera.transform.rotation;
        Quaternion newRotation = otherPortal.transform.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;

        // Apply the new position and rotation
        portalCamera.transform.SetPositionAndRotation(newCameraPosition, newRotation);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}