using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsDay3 : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void FlameMovesTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 3 && !battleEvents.tutFlameMoves.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutFlameMoves.flag = true;
        }
    }

    public void BurnTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 3 && !battleEvents.tutBurn.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutFlameMoves.flag = true;
        }
    }
}
