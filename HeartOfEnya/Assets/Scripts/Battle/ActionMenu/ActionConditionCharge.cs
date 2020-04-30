using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionCharge : ActionCondition
{
    public bool enableWhenCharged;
    public bool enableWhenCharging;
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        if(user.IsChargingAction)
        {
            return enableWhenCharged;
        }
        if (enableWhenCharged)
            return false;
        return !enableWhenCharging;

    }
}
