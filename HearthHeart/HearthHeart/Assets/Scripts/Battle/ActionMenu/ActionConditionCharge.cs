using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionCharge : ActionCondition
{
    public bool enableWhenCharged;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        if (user.ChargingActionReady)
            return enableWhenCharged;
        return !enableWhenCharged;
    }
}
