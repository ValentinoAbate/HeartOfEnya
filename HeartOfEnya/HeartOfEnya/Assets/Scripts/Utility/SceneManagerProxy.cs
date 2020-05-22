using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerProxy : MonoBehaviour
{
    public void TransitionScenes(string sceneName)
    {
        SceneTransitionManager.main.TransitionScenes(sceneName);
    }
}
