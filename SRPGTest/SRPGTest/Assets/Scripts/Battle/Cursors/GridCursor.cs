using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCursor : Cursor
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = BattleGrid.main.GetSpace(Pos);
        Highlight();
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;
        var obj = BattleGrid.main.GetObject(newPos);
        if (obj != null && obj.ObjectType == FieldObject.ObjType.Obstacle)
            return;

        UnHighlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        Highlight();
    }

    public override void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Pos);
        if (highlighted != null)
        {
            if (highlighted.Select())
                gameObject.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Select();
        }
    }

    public void Highlight()
    {
        BattleGrid.main.GetObject(Pos)?.Highlight();
    }

    public void UnHighlight()
    {
        BattleGrid.main.GetObject(Pos)?.UnHighlight();
    }
}
