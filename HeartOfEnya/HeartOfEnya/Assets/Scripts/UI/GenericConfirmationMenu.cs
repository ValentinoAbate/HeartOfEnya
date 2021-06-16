using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GenericConfirmationMenu : MonoBehaviour
{
    public enum Mode
    {
        ChangeScene,
        Quit,
        None,
    }

    public System.Action OnConfirm { get; set; }

    [SerializeField] private Mode mode = Mode.None;
    [SerializeField] private string scene;

    public void SetQuitMode()
    {
        mode = Mode.Quit;
    }

    public void SetSceneChangeMode()
    {
        mode = Mode.ChangeScene;
    }

    public void SetScene(string scene)
    {
        this.scene = scene;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        OnConfirm?.Invoke();
        if(mode == Mode.Quit)
        {
            SceneTransitionManager.main.doExitGame();
        }
        else
        {
            if(scene == "MainMenu")
            {
                DoNotDestroyOnLoad.Instance?.ResetPersistantData();
            }
            SceneTransitionManager.main.TransitionScenes(scene);
        }
    }
}
