using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNextCursor : Cursor
{
    public KeyCode nextKey;
    public KeyCode lastKey;
    public FieldObject Selected { get => SelectionList[selectedInd]; }
    public bool Empty => SelectionList.Count <= 0;
    protected List<FieldObject> SelectionList { get; set; } = new List<FieldObject>();
    protected int selectedInd = 0;

    public void SetSelected(IEnumerable<FieldObject> objects)
    {
        SelectionList.Clear();
        SelectionList.AddRange(objects);
    }

    public void RemovedCurrentSelection()
    {
        SelectionList.RemoveAt(selectedInd);
        selectedInd--;
    }

    public void HighlightFirst()
    {
        selectedInd = -1;
        HighlightNext();
        
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;
        BattleGrid.main.GetObject(Pos)?.UnHighlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        BattleGrid.main.GetObject(Pos)?.Highlight();
    }

    public override void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Pos);
        if (highlighted != null)
        {
            if(highlighted.Select())
            {
                highlighted.StartTurn();
                SetActive(false);
            }
        }
    }

    public void HighlightNext()
    {
        if (Empty)
            return;
        SelectionList.RemoveAll((obj) => obj == null);
        if (++selectedInd >= SelectionList.Count)
            selectedInd = 0;
        Highlight(SelectionList[selectedInd].Pos);
    }

    public void HighlightPrev()
    {
        if (Empty)
            return;
        if (--selectedInd < 0)
            selectedInd = SelectionList.Count - 1;
        Highlight(SelectionList[selectedInd].Pos);
    }

    public override void ProcessInput()
    {
        if (Input.GetKeyDown(nextKey))
        {
            HighlightNext();
        }
        else if (Input.GetKeyDown(lastKey))
        {
            HighlightPrev();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Select();
        }
    }
}
