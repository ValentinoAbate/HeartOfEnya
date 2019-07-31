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
        return BattleGrid.main.SearchArea(cursor.attacker.Pos, range, cursor.CanMoveThrough, IsTarget);
    }

    private bool IsTarget(FieldObject obj)
    {
        return !(obj == null || obj == cursor.attacker || cursor.ignore.Any((t) => t == obj.Allegiance));
    }
}
