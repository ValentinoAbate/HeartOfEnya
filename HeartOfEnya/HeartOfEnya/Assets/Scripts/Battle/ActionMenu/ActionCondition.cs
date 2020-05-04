using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionCondition : MonoBehaviour
{
    public enum OnConditionFail
    {
        Hide,
        Disable,
    }

    public bool invert;

    public OnConditionFail onConditionFail;

    public bool CheckCondition(ActionMenu menu, PartyMember user)
    {
        return invert ? !CheckConditionFn(menu, user) : CheckConditionFn(menu, user);
    }

    protected abstract bool CheckConditionFn(ActionMenu menu, PartyMember user);
}
