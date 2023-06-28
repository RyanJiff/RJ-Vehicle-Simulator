using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /*
     * Handles main menu canvas
     */

    private enum ActiveMenu { MAIN_MENU, SETTINGS_MENU};
    [SerializeField] private ActiveMenu activeMenu = ActiveMenu.MAIN_MENU;

    // Menus
    [Header("Main Menu")]
    [SerializeField] private Transform mainMenu = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private Button exitApplicationButton = null;
    [Space]

    [Header("Settings Menu")]
    [SerializeField] private Transform settingsMenu = null;
    [SerializeField] private Button backButton = null;
    
    private void Start()
    {
        InitializeMainMenu();
    }
    private void Update()
    {
        mainMenu.gameObject.SetActive(activeMenu == ActiveMenu.MAIN_MENU);
        settingsMenu.gameObject.SetActive(activeMenu == ActiveMenu.SETTINGS_MENU);
    }
    private void InitializeMainMenu()
    {
        if (ApplicationManager.instance != null)
        {
            SwitchActiveMenu(ActiveMenu.MAIN_MENU);

            // Main Menu
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(() => ApplicationManager.instance.LoadGameScene());
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => SwitchActiveMenu(ActiveMenu.SETTINGS_MENU));
            exitApplicationButton.onClick.RemoveAllListeners();
            exitApplicationButton.onClick.AddListener(() => ApplicationManager.instance.QuitApplication());

            // Settings Menu
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => SwitchActiveMenu(ActiveMenu.MAIN_MENU));
        }
        else
        {
            Debug.LogError("APPLICATION MANAGER NOT SET, MAIN MENU WILL NOT WORK.");
            this.enabled = false;
        }
    }
    private void SwitchActiveMenu(ActiveMenu aM)
    {
        activeMenu = aM;
    }
}
