/// <summary>
/// Manages the settings menu on the player's wrist
/// </summary>

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menu Prefab")]
    [SerializeField] private GameObject menuPrefab;
    private GameObject menuInstance;

    [Header("XR Components")]
    public InputActionProperty showButton;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;
    public Transform leftHandController;

    [Header("Menu Transform Settings")]
    private Vector3 menuPositionOffset = new Vector3(-0.95f, -0.65f, 0f);
    private Vector3 menuRotationOffset = new Vector3(0f, -95f, 0f);
    private Vector3 menuScale = new Vector3(0.3f, 0.3f, 0.05f);

    private bool isMenuActive = false;
    private MenuNavigator menuNavigator;

    void Start()
    {
        InstantiateMenu();
        SetRayInteractorsActive(false);
    }

    private void InstantiateMenu()
    {
        if (menuPrefab == null || leftHandController == null)
        {
            Debug.LogError("Menu prefab or left hand controller not assigned!");
            return;
        }

        menuInstance = Instantiate(menuPrefab, leftHandController);
        menuInstance.SetActive(false);
        UpdateMenuTransform();

        // Get reference to the MenuNavigator component
        menuNavigator = menuInstance.GetComponentInChildren<MenuNavigator>();
        if (menuNavigator == null)
        {
            Debug.LogError("MenuNavigator component not found in prefab!");
        }
    }

    // oggle when player presses menu button
    void Update()
    {
        UpdateMenuTransform();
        if (showButton.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    // Set scale and position to be located on player's left wrist
    private void UpdateMenuTransform()
    {
        if (menuInstance != null)
        {
            menuInstance.transform.localPosition = menuPositionOffset;
            menuInstance.transform.localRotation = Quaternion.Euler(menuRotationOffset);
            menuInstance.transform.localScale = menuScale;
        }
    }

    // Set menu to be visible or not visible
    private void ToggleMenu()
    {
        if (menuInstance == null || menuNavigator == null) return;

        isMenuActive = !isMenuActive;
        menuInstance.SetActive(isMenuActive);
        
        if (isMenuActive)
        {
            menuNavigator.NavigateToGameMenu();
        }
        
        SetRayInteractorsActive(isMenuActive);
    }

    // Activates ray interactors for both hands
    private void SetRayInteractorsActive(bool active)
    {
        if (leftHandRay != null) leftHandRay.gameObject.SetActive(active);
        if (rightHandRay != null) rightHandRay.gameObject.SetActive(active);
    }
}