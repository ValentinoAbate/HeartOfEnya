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

    public OnConditionFail onConditionFail;
    public abstract bool CheckCondition(PartyMember user);
}
