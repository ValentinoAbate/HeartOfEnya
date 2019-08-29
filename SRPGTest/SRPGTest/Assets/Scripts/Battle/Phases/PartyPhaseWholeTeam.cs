using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhaseWholeTeam : PartyPhase
{
    public override Cursor Cursor { get => cursor; }
    [SerializeField]
    private SelectNextCursor cursor;
    protected int selected;

    public override Coroutine OnPhaseEnd()
    {
        cursor.gameObject.SetActive(false);
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
        // Set the cursor's selection list to the party members with turns
        cursor.SetSelected(activeParty);
        cursor.HighlightFirst();
        cursor.SetActive(true);
        return null;
    }

    public override void EndAction(PartyMember p)
    {
        cursor.RemoveCurrentSelection();
        cursor.RemoveAll((obj) => obj == null || !((obj as PartyMember).HasTurn));
        if (cursor.Empty)
            EndPhase();
        else
        {
            cursor.HighlightNext();
            cursor.SetActive(true);
        }
    }

    public override void OnPhaseUpdate() { }
}
