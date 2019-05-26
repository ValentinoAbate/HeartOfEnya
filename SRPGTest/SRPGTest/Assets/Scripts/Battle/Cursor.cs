using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public BattleGrid.Pos Pos { get => new BattleGrid.Pos(Row, Col); }
    public int Row { get; set; }
    public int Col { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        transform.position = BattleGrid.main.GetSpace(Row, Col);
        Highlight();
    }

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public virtual void Highlight(int row, int col)
    {
        if (row == Row && col == Col)
            return;
        if (!BattleGrid.main.IsLegal(row, col))
            return;
        var obj = BattleGrid.main.GetObject(row, col);
        if (obj != null && obj.ObjectType == FieldObject.ObjType.Obstacle)
            return;

        UnHighlight();
        Row = row;
        Col = col;
        transform.position = BattleGrid.main.GetSpace(Row, Col);
        Highlight();
    }

    public virtual void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Row, Col);
        if (highlighted != null)
        {
            if (highlighted.Select())
                gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        ProecessInput();
        if (Input.GetKeyDown(KeyCode.E))
        {
            (PhaseManager.main.ActivePhase as PartyPhase)?.SelectNext();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            (PhaseManager.main.ActivePhase as PartyPhase)?.SelectPrev();
        }
    }

    public virtual void ProecessInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Highlight(Row - 1, Col);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Highlight(Row + 1, Col);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Highlight(Row, Col - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Highlight(Row, Col + 1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Select();
        }
    }

    public void Highlight()
    {
        BattleGrid.main.GetObject(Row, Col)?.Highlight();
    }

    public void UnHighlight()
    {
        BattleGrid.main.GetObject(Row, Col)?.UnHighlight();
    }
}
