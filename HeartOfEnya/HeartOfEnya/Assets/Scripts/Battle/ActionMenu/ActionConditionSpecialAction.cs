using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionSpecialAction : ActionCondition
{
    public ActionMenu.SpecialAction action;
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        return user.ActionMenu.IsSpecialActionEnabled(action);
    }
}
