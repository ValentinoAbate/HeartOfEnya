using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionConditionSingleTarget : ActionCondition
{
    public Action action;
    public AttackCursor cursor;
    public override bool CheckCondition(Combatant user)
    {
        cursor.SetAction(action);
        cursor.CalculateTargets();
        return !cursor.Empty;
    }
}
