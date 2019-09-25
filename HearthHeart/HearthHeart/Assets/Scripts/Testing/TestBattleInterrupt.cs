using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBattleInterrupt : MonoBehaviour
{
    public KeyCode testKey;
    public string startNode;
    public Yarn.Unity.DialogueRunner runner;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(testKey))
        {
            runner.StartDialogue(startNode);
        }
    }
}
