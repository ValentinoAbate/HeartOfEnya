using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionNotOnDeathsDoor : ActionCondition
{
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return !user.DeathsDoor;
    }
}
