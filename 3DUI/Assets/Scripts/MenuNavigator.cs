/// <summary>
/// Manages navigation to each screen in the settings menu
/// </summary>

using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    // References to menu screens (assign these in the prefab inspector)
    [Header("Menu Screens")]
    public GameObject gameMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public GameObject audioMenu;
    public GameObject languageMenu;

    // Navigation functions that can be called directly from UI buttons
    public void NavigateToGameMenu()
    {
        ShowMenu(gameMenu);
    }

    public void NavigateToSettingsMenu()
    {
        ShowMenu(settingsMenu);
    }

    public void NavigateToControlsMenu()
    {
        ShowMenu(controlsMenu);
    }

    public void NavigateToAudioMenu()
    {
        ShowMenu(audioMenu);
    }

    public void NavigateToLanguageMenu()
    {
        ShowMenu(languageMenu);
    }

    private void ShowMenu(GameObject menuToShow)
    {
        // Deactivate all menus first
        gameMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        audioMenu.SetActive(false);
        languageMenu.SetActive(false);

        // Activate the selected menu
        menuToShow.SetActive(true);
    }
}