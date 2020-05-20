using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A cursor that can be moved around the grid.
/// Current
/// </summary>
public class GridCursor : Cursor
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Highlight(Pos);
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;

        BattleGrid.main.Get<FieldObject>(Pos)?.Highlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        BattleGrid.main.Get<FieldObject>(Pos)?.UnHighlight();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Select();
        }
    }
}
