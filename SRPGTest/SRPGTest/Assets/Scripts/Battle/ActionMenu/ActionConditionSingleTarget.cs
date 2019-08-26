using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionConditionSingleTarget : ActionCondition
{
    public ActiveAbility ability;
    public AttackCursor cursor;
    public override bool CheckCondition()
    {
        cursor.SetAttack(ability);
        cursor.CalculateTargets();
        return !cursor.Empty;
    }
}
