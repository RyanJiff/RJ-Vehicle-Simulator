using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    /*
     * The application manager handles scene management and game level loading.
     * 
     * Scene 0 in the build index is an empty scene with an application manager instance that loads scene 1. Scene 1 will contain the main menu.
     */
    public static ApplicationManager instance { get; private set; }

    // Scene index constants.
    public const int SCENES_MAINMENU = 1;
    public const int SCENES_GAME = 2;

    public LevelData levelData = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Get out of the first loaded scene to the main menu
        SceneManager.LoadScene(SCENES_MAINMENU);
    }
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(SCENES_MAINMENU);
    }
    public void LoadGameScene()
    {
        if (levelData != null)
        {
            SceneManager.LoadScene(SCENES_GAME);
        }
        else
        {
            Debug.Log("No level data set!");
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == SCENES_MAINMENU)
        {
            // We just loaded the main menu.
        }
        else if(scene.buildIndex == SCENES_GAME)
        {
            // We just loaded the game scene.
            Instantiate(levelData.worldPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(levelData.environmentSystemPrefab, Vector3.zero, Quaternion.identity);
            Vehicle v = Instantiate(levelData.playerStartingVehicle, levelData.playerVehicleStartingPosition, Quaternion.identity).GetComponent<Vehicle>();
            Instantiate(levelData.playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>().TakeControl(v);
        }
    }
    public void QuitApplication()
    {
        Application.Quit(0);
    }
}
