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
        selectNextCursor.SelectionList.Clear();
        selectNextCursor.SelectionList.AddRange(Party);
        Party.ForEach((p) => p.OnPhaseStart());
        selectNextCursor.HighlightFirst();
        selectNextCursor.SetActive(true);
        return null;
    }

    public override void EndAction(PartyMember p)
    {
        selectNextCursor.SelectionList.Remove(p);
        if (selectNextCursor.SelectionList.Count <= 0)
            EndPhase();
        else
        {
            selectNextCursor.HighlightNext();
            selectNextCursor.SetActive(true);
        }
    }

    public override void OnPhaseUpdate() { }
}
