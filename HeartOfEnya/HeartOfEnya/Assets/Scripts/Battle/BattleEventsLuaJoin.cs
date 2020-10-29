using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsLuaJoin : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void Move3Trigger()
    {
        if(battleEvents.tutLuaJoin && !battleEvents.abs0Battle && !battleEvents.tutMove3.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Move 3");
            battleEvents.tutMove3.flag = true;

            StartCoroutine(Move3Post(DialogManager.main.runner));
        }
    }

    private IEnumerator Move3Post(DialogueRunner runner)
    {
        runner.StartDialogue("TutMove3");
        yield return new WaitWhile(() => runner.isDialogueRunning);

        battleEvents.Unpause();
    }

    public void LuaTrigger()
    {
        if(battleEvents.tutLuaJoin && !battleEvents.abs0Battle && !battleEvents.tutLua.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Lua");
            battleEvents.tutLua.flag = true;

            StartCoroutine(LuaPost(DialogManager.main.runner));
        }
    }

    private IEnumerator LuaPost(DialogueRunner runner)
    {
        runner.StartDialogue("TutLuaSelect");
        yield return new WaitWhile(() => runner.isDialogueRunning);

        battleEvents.Unpause();
    }
}
