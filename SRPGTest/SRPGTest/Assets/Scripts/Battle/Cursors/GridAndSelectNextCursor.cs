using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAndSelectNextCursor : SelectNextCursor
{

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
