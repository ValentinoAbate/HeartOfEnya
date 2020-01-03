using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionCanEnterFlameMode : ActionCondition
{
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return menu.allowFlameMode;
    }
}
