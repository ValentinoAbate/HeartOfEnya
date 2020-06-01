using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager main;
    public Animator fade;
    public float fadeTime = 1f;
    private bool click = false;
    
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
        // If scene is Battle, do the special to battle transition
        // If scene is camp, campIntro, or campOutro, do the special to camp transition
        // else do the normal fade
        if (sceneName == "Battle")
        {
            StartCoroutine(BattleTransition(sceneName));
        }else if(sceneName == "Camp" || sceneName == "OutroCamp" || sceneName == "IntroCamp")
        {
            StartCoroutine(CampTransition(sceneName));
        }
        else
        {
            StartCoroutine(StartFade(sceneName));
        }

    }

    IEnumerator BattleTransition(string name)
    {
        Debug.Log("Battle Transition");
        if (!(click))
        {
            click = true;
            fade.SetTrigger("Start");
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene(name);
            fade.SetTrigger("End");
            click = false;
        }
        
    }
    IEnumerator CampTransition(string name)
    {
        Debug.Log("Camp Transition");
        if (!(click))
        {
            click = true;
            fade.SetTrigger("Start");
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene(name);
            fade.SetTrigger("End");
            click = false;
        }

    }
    IEnumerator StartFade(string name)
    {
        Debug.Log("Fade Transition");
        if (!(click))
        {
            click = true;
            fade.SetTrigger("Start");
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene(name);
            fade.SetTrigger("End");
            click = false;
        }

    }
    /// <summary>
    /// Exit Functionality for Exit Button
    /// </summary>
    public void doExitGame()
    {
        Application.Quit();
    }
}
