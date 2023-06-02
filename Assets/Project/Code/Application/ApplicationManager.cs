using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    /*
     * The application manager handles scene management.
     * As of right now scene 0 in the build index is an empty scene with an application manager instance that loads scene 1.
     */
    public static ApplicationManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // We already have an instance
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        // Get out of the first loaded scene.
        SceneManager.LoadScene(1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
