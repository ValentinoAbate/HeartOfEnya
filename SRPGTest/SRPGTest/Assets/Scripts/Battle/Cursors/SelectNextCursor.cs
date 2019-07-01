using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNextCursor : Cursor
{
    public FieldObject Selected { get => SelectionList[selected]; }
    public List<FieldObject> SelectionList { get; set; } = new List<FieldObject>();
    private int selected = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = BattleGrid.main.GetSpace(Pos);
        Highlight();
    }

    public void HighlightFirst()
    {
        selected = -1;
        HighlightNext();
        
    }

    public override void Highlight(Pos newPos)
    {
        BattleGrid.main.GetObject(Pos)?.UnHighlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        BattleGrid.main.GetObject(Pos)?.UnHighlight();
    }

    public override void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Pos);
        if (highlighted != null)
        {
            highlighted.StartTurn();
            SetActive(false);
        }
    }

    public void HighlightNext()
    {
        if (++selected >= SelectionList.Count)
            selected = 0;
        Highlight(SelectionList[selected].Pos);
    }

    public void HighlightPrev()
    {
        if (--selected < 0)
            selected = SelectionList.Count - 1;
        Highlight(SelectionList[selected].Pos);
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }

    public override void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            HighlightNext();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            HighlightPrev();
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
