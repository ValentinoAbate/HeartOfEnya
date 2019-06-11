using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionConditionSingleTarget : ActionCondition
{
    public int range;
    public SingleTargetAttackCursor cursor;
    public override bool CheckCondition()
    {
        return BattleGrid.main.SearchArea(cursor.attacker.Pos, range, IsTarget, cursor.blockedBy);
    }

    private bool IsTarget(FieldObject obj)
    {
        return !(obj == null || cursor.ignore.Any((t) => t == obj.ObjectType));
    }
}
