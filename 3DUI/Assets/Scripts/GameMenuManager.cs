using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;
    
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    private bool isMenuActive = false;

    void Start()
    {
        menu.SetActive(false);
        SetRayInteractorsActive(false);
    }

    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        if (isMenuActive)
        {
            UpdateMenuPosition();
        }
    }

    private void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        menu.SetActive(isMenuActive);

        if (isMenuActive)
        {
            // Use the actual forward direction of the head
            Vector3 headForward = head.forward;
            
            // Calculate the spawn position
            Vector3 spawnPosition = head.position + headForward * spawnDistance;
            
            // Ensure the menu is at eye level
            spawnPosition.y = head.position.y + 0.5f;
            
            // Set the menu position
            menu.transform.position = spawnPosition;
            
            // Make the menu face the user
            menu.transform.LookAt(head);
            menu.transform.forward *= -1;
        }

        SetRayInteractorsActive(isMenuActive);
    }

    private void UpdateMenuPosition()
    {
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }

    private void SetRayInteractorsActive(bool active)
    {
        if (leftHandRay != null)
        {
            leftHandRay.gameObject.SetActive(active);
        }
        
        if (rightHandRay != null)
        {
            rightHandRay.gameObject.SetActive(active);
        }
    }
}