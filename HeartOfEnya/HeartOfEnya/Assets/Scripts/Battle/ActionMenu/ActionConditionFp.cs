using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionFp : ActionCondition
{
    public int fpCost = 1;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return user.Fp >= fpCost;
    }
}
