using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionFp : ActionCondition
{
    public int fpCost = 1;
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        return user.Fp >= fpCost;
    }
}
