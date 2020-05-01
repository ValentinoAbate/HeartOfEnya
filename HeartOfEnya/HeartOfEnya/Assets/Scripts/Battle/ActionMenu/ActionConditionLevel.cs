using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionLevel : ActionCondition
{
    public int levelReq;
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        return user.level >= levelReq;
    }
}

