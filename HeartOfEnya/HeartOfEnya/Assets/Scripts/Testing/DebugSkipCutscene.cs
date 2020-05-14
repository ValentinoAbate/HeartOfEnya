using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSkipCutscene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DialogueManager.main.runner.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete) && DialogueManager.main.runner.isDialogueRunning)
        {
            DialogueManager.main.runner.Stop();

        }
    }
}
