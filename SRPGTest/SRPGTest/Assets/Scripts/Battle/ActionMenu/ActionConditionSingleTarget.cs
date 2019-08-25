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

    private bool IsTarget(FieldObject obj)
    {
        return !(obj == null || obj == cursor.attacker || cursor.ignore.Any((t) => t == obj.Allegiance));
    }
}
