using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionCharge : ActionCondition
{
    public bool enableWhenCharged;
    public bool enableWhenCharging;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        if(user.IsChargingAction)
        {
            if (user.ChargingActionReady)
                return enableWhenCharged;
            return enableWhenCharging;
        }
        if (enableWhenCharged)
            return false;
        return !enableWhenCharging;

    }
}
