using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager main;
    
    /// <summary>
    /// Implements the singleton pattern
    /// </summary>
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Changes to the scene specified by given scene name
    /// </summary>
    public void TransitionScenes(string sceneName)
    {
    	SceneManager.LoadScene(sceneName);
    }
}
