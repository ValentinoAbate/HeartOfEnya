using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionConditionAnd : ActionCondition
{
    public List<ActionCondition> conditions;
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        return conditions.All((c) => c.CheckCondition(menu, user));
    }
}
