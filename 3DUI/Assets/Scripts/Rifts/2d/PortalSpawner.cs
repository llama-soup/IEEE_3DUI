using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    public GameObject presentWire;
    public GameObject futureWire;
    public GameObject presentCatcard;
    public GameObject futureCatcard;

    private void OnTriggerEnter(Collider other)
    {
        // If wires are active, deactivate them
        if (presentWire.activeSelf)
        {
            presentWire.SetActive(false);
            futureWire.SetActive(false);
            
            // Activate catcards
            presentCatcard.SetActive(true);
            futureCatcard.SetActive(true);
        }
    }
}