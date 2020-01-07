using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A selection list cursor that can also be moved around the grid like a GridCursor
/// </summary>
public class GridAndSelectionListCursor : SelectionListCursor
{
    /// <summary>
    /// Basic Highlighting logic that also updates the selcted object in the selction list to the highlighted object if applicable.
    /// </summary>
    public override void Highlight(Pos newPos)
    {
        base.Highlight(newPos);
        var highlightedObj = BattleGrid.main.GetObject(Pos);
        if (highlightedObj != null)
        {
            int index = SelectionList.IndexOf(highlightedObj);
            if (index != -1)
                selectedInd = index;
        }
    }

    /// <summary>
    /// Adds grid cursor logic to inherited selection cursor logic
    /// </summary>
    public override void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Highlight(Pos + Pos.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Highlight(Pos + Pos.Down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Highlight(Pos + Pos.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Highlight(Pos + Pos.Right);
        }
        base.ProcessInput();
    }
}
