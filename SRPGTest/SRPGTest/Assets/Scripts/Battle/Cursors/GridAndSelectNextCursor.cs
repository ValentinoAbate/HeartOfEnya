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
            Highlight(Pos.Offset(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Highlight(Pos.Offset(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Highlight(Pos.Offset(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Highlight(Pos.Offset(0, 1));
        }
        base.ProcessInput();
    }
}
