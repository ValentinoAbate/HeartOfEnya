using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTileAction : EventTile
{
    public ActionMenu.SpecialAction action;

    protected override void OnSteppedOnFn(FieldObject obj)
    {
        var actionMenu = (obj as PartyMember)?.ActionMenu;
        if (actionMenu == null)
            return;
        actionMenu.EnableSpecialAction(action);
    }

    protected override void OnLeaveTileFn(FieldObject obj)
    {
        var actionMenu = (obj as PartyMember)?.ActionMenu;
        if (actionMenu == null)
            return;
        actionMenu.DisableSpecialAction(action);
    }
}
