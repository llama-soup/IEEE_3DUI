/// <summary>
/// Manages spawning and despawning portals
/// </summary>

using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    public GameObject presentWire;
    public GameObject futureWire;
    public GameObject presentCatcard;
    public GameObject futureCatcard;

    private void OnTriggerEnter(Collider other)
    {
        // If player enters room and wire portals are active, deactivate them and activate catcard portals
        if (presentWire.activeSelf)
        {
            presentWire.SetActive(false);
            futureWire.SetActive(false);
            
            presentCatcard.SetActive(true);
            futureCatcard.SetActive(true);
        }
    }
}