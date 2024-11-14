using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalCamera : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera portalACam;
    [SerializeField] private Camera portalBCam;

    [Header("Portal A")]
    [SerializeField] private Transform portalA;
    [SerializeField] private MeshRenderer portalSurfaceA;
    
    [Header("Portal B")]
    [SerializeField] private Transform portalB;
    [SerializeField] private MeshRenderer portalSurfaceB;
    
    private Camera playerCamera;
    private RenderTexture portalTextureA;
    private RenderTexture portalTextureB;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        
        // Create render textures with double width for stereo rendering
        portalTextureA = new RenderTexture(Screen.width * 2, Screen.height, 24, RenderTextureFormat.ARGB32);
        portalTextureB = new RenderTexture(Screen.width * 2, Screen.height, 24, RenderTextureFormat.ARGB32);
        
        // Set up Portal A camera
        portalACam.targetTexture = portalTextureA;
        portalACam.stereoTargetEye = StereoTargetEyeMask.Both;
        
        // Set up Portal B camera
        portalBCam.targetTexture = portalTextureB;
        portalBCam.stereoTargetEye = StereoTargetEyeMask.Both;
        
        // Each portal shows what its camera sees
        portalSurfaceA.material.mainTexture = portalTextureA; // A shows what A's camera sees (from B's position)
        portalSurfaceB.material.mainTexture = portalTextureB; // B shows what B's camera sees (from A's position)

        Debug.Log("Portal Camera System Initialized");
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        // Only update if it's the player camera
        if (camera != playerCamera)
            return;

        // Update Portal A's camera (positioned at Portal B, looking back)
        UpdatePortalCamera(portalACam, portalA, portalB);
        
        // Update Portal B's camera (positioned at Portal A, looking back)
        UpdatePortalCamera(portalBCam, portalB, portalA);
        
        // Render the views
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalACam);
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalBCam);
    }

    private void UpdatePortalCamera(Camera portalCam, Transform viewedPortal, Transform targetPortal)
    {
        // Position camera at the target portal
        portalCam.transform.position = targetPortal.position;

        // Calculate angle between player and viewed portal
        Vector3 directionToPlayer = transform.position - viewedPortal.position;
        float angleToPlayer = Vector3.SignedAngle(viewedPortal.forward, directionToPlayer, Vector3.up);

        // Set camera rotation to look back through the portal
        portalCam.transform.rotation = targetPortal.rotation * Quaternion.Euler(0, angleToPlayer, 0);

        // Debug visualization
        Debug.DrawLine(viewedPortal.position, transform.position, Color.yellow);
        Debug.DrawRay(portalCam.transform.position, portalCam.transform.forward * 2, Color.green);
    }

    private void OnDrawGizmos()
    {
        if (portalA != null && portalB != null)
        {
            // Draw portal directions
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(portalA.position, portalA.forward * 2);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(portalB.position, portalB.forward * 2);
            
            // Draw portal positions
            if (portalACam != null && portalBCam != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(portalACam.transform.position, 0.5f);
                Gizmos.DrawWireSphere(portalBCam.transform.position, 0.5f);
                
                // Draw lines from player to portal cameras if in play mode
                if (Application.isPlaying)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, portalACam.transform.position);
                    Gizmos.DrawLine(transform.position, portalBCam.transform.position);
                }
            }

            // Draw portal frames
            Gizmos.color = Color.white;
            DrawPortalFrame(portalA);
            DrawPortalFrame(portalB);
        }
    }

    private void DrawPortalFrame(Transform portal)
    {
        Vector3 topLeft = portal.position + portal.up * 1f - portal.right * 0.5f;
        Vector3 topRight = portal.position + portal.up * 1f + portal.right * 0.5f;
        Vector3 bottomLeft = portal.position - portal.up * 1f - portal.right * 0.5f;
        Vector3 bottomRight = portal.position - portal.up * 1f + portal.right * 0.5f;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private void OnDestroy()
    {
        // Clean up render textures
        if (portalTextureA != null)
            portalTextureA.Release();
        if (portalTextureB != null)
            portalTextureB.Release();
    }
}