using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionFlameMode : ActionCondition
{
    public bool enableInFlameMode;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        if (menu.FlameMode)
            return enableInFlameMode;
        return !enableInFlameMode;
    }
}
