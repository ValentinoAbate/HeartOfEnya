using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionSpecialAction : ActionCondition
{
    public ActionMenu.SpecialAction action;
    public override bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return user.ActionMenu.IsSpecialActionEnabled(action);
    }
}
