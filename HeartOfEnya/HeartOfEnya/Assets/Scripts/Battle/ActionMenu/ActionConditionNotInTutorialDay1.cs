public class ActionConditionNotInTutorialDay1 : ActionCondition
{
    protected override bool CheckConditionFn(ActionMenu menu, PartyMember user)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        return !pData.InTutorialFirstDay;
    }
}
