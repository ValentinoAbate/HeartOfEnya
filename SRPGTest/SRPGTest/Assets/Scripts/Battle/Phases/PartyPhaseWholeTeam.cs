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
        Party.ForEach((member) => member.OnPhaseEnd());
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        // Remove all dead party members
        Party.RemoveAll((p) => p == null);
        Party.ForEach((p) => p.OnPhaseStart());
        // The party members that can act this turn
        var activeParty = new List<PartyMember>(Party);
        // Remove party members that are stunned or otherwise don't have a turn
        activeParty.RemoveAll((p) => p.Stunned || !p.HasTurn);
        activeParty.Sort((p1, p2) => Pos.CompareLeftToRightTopToBottom(p1.Pos, p2.Pos));
        selectNextCursor.SetSelected(activeParty);
        selectNextCursor.HighlightFirst();
        selectNextCursor.SetActive(true);
        return null;
    }

    public override void EndAction(PartyMember p)
    {
        selectNextCursor.RemoveCurrentSelection();
        selectNextCursor.RemoveAll((obj) => obj == null || !((obj as PartyMember).HasTurn));
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
