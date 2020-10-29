using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionNotOnDeathsDoor : ActionCondition
{
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        return !user.DeathsDoor;
    }
}
