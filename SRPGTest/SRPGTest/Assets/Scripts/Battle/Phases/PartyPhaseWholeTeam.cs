using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhaseWholeTeam : PartyPhase
{
    public override Cursor Cursor { get => selectNextCursor; }
    [SerializeField]
    private SelectNextCursor selectNextCursor;
    protected int selected;

    public override Coroutine OnPhaseEnd()
    {
        selectNextCursor.gameObject.SetActive(false);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Party.RemoveAll((e) => e == null);
        Party.Sort((p1, p2) =>
        {
            int colComp = p1.Pos.col.CompareTo(p2.Pos.col);
            if (colComp != 0)
                return colComp;
            return p1.Pos.row.CompareTo(p2.Pos.row);
        }
        );
        selectNextCursor.SetSelected(Party);
        Party.ForEach((p) => p.OnPhaseStart());
        selectNextCursor.HighlightFirst();
        selectNextCursor.SetActive(true);
        return null;
    }

    public override void EndAction(PartyMember p)
    {
        selectNextCursor.RemovedCurrentSelection();
        if (selectNextCursor.Empty)
            EndPhase();
        else
        {
            selectNextCursor.HighlightNext();
            selectNextCursor.SetActive(true);
        }
    }

    public override void OnPhaseUpdate() { }
}
