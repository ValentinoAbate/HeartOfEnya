using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject[] toPause;
    public GameObject pauseUI;
    public KeyCode pauseKey;
    private bool pause = false;

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(pauseKey))
       {
            pause = !pause;
            pauseUI.SetActive(pause);
            foreach (var obj in toPause)
            {
                var pausables = obj.GetComponents<IPausable>();
                foreach (var pausable in pausables)
                    pausable.PauseHandle.SetPause(pause, PauseHandle.PauseSource.PauseMenu);
            }
       }
    }
}
