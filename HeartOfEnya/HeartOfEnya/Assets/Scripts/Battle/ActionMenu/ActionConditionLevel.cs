using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionLevel : ActionCondition
{
    public int levelReq;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return user.level >= levelReq;
    }
}

