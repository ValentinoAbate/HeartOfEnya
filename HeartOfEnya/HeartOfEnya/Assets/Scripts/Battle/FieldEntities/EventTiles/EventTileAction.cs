using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An EventTile that enables a special action to be used when a unit steps on it
/// Example: run tiles use this to enable running from battle, but it can also be used for talking, interacting, etc.
/// </summary>
public class EventTileAction : EventTile
{
    /// <summary>
    /// The action eneabled by this tile
    /// </summary>
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
