using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsLuaJoin : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void Move3Trigger()
    {
        if(battleEvents.luaUnfrozen && !battleEvents.abs0Battle && !battleEvents.tutMove3.flag)
        {
            Debug.Log("Battle Triggers: Move 3");
            battleEvents.tutMove3.flag = true;

            StartCoroutine(Move3Post(DialogueManager.main.runner));
        }
    }

    private IEnumerator Move3Post(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void LuaTrigger()
    {
        if(battleEvents.luaUnfrozen && !battleEvents.abs0Battle && !battleEvents.tutLua.flag)
        {
            Debug.Log("Battle Triggers: Lua");
            battleEvents.tutLua.flag = true;

            StartCoroutine(LuaPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator LuaPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
