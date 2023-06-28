using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    /*
     * Handles pause menu canvas
     */

    private enum ActiveMenu { PAUSE_MENU, SETTINGS_MENU };
    [SerializeField] private ActiveMenu activeMenu = ActiveMenu.PAUSE_MENU;

    [Header("Pause Menu")]
    [SerializeField] private Transform pauseMenu = null;
    [SerializeField] private Button resumeGameButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private Button returnToMainMenuButton = null;
    [Space]

    [Header("Settings Menu")]
    [SerializeField] private Transform settingsMenu = null;
    [SerializeField] private Button backButton = null;

    private void Start()
    {
        InitializePauseMenu();
    }
    private void Update()
    {
        pauseMenu.gameObject.SetActive(activeMenu == ActiveMenu.PAUSE_MENU);
        settingsMenu.gameObject.SetActive(activeMenu == ActiveMenu.SETTINGS_MENU);
    }
    private void InitializePauseMenu()
    {
        SwitchActiveMenu(ActiveMenu.PAUSE_MENU);

        // Main Menu
        resumeGameButton.onClick.RemoveAllListeners();
        resumeGameButton.onClick.AddListener(() => FindObjectOfType<PlayerController>().TogglePause());
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(() => SwitchActiveMenu(ActiveMenu.SETTINGS_MENU));
        returnToMainMenuButton.onClick.RemoveAllListeners();
        if (ApplicationManager.instance != null)
        {
            returnToMainMenuButton.onClick.AddListener(() => ApplicationManager.instance.LoadMainMenuScene());
        }
        else
        {
            returnToMainMenuButton.onClick.AddListener(() => Debug.Log("We would have went back to the main menu if we had an application manager instance."));
        }

        // Settings Menu
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => SwitchActiveMenu(ActiveMenu.PAUSE_MENU));
    }
    private void SwitchActiveMenu(ActiveMenu aM)
    {
        activeMenu = aM;
    }
}
