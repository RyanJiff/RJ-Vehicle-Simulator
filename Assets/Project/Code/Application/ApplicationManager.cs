using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    /*
     * The application manager handles scene management.
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
