using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMenuManager : MonoBehaviour
{
    public GameObject menu;
    public InputActionProperty showButton;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;
    public Transform leftHandController;

    private Vector3 menuPositionOffset = new Vector3(0f, 0.2f, -0.15f);
    private Vector3 menuRotationOffset = new Vector3(0f, -95f, 0f);
    private Vector3 menuScale = new Vector3(0.00025f, 0.00025f, 0.00025f);

    private bool isMenuActive = false;

    void Start()
    {
        menu.SetActive(false);
        SetRayInteractorsActive(false);
        AttachMenuToWrist();
    }

    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        if (isMenuActive)
        {
            UpdateMenuTransform();
        }
    }

    private void AttachMenuToWrist()
    {
        menu.transform.SetParent(leftHandController, false);
        UpdateMenuTransform();
    }

    private void UpdateMenuTransform()
    {
        menu.transform.localPosition = menuPositionOffset;
        menu.transform.localRotation = Quaternion.Euler(menuRotationOffset);
        menu.transform.localScale = menuScale;
    }

    private void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        menu.SetActive(isMenuActive);
        SetRayInteractorsActive(isMenuActive);
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