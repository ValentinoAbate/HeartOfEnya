using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; }
    public Cursor Cursor { get => cursor; }

    public List<PartyMember> Party { get; } = new List<PartyMember>();

    private void Awake()
    {
        PauseHandle = new PauseHandle(null, cursor);
    }


    [SerializeField]
    private SelectNextCursor cursor;
    private int selected;

    public override Coroutine OnPhaseStart()
    {
        // Remove all dead party members
        Party.RemoveAll((p) => p == null);
        // Call on phse start function for each party member (may need to wait later for DOT effects, stunning, etc.)
        Party.ForEach((p) => p.OnPhaseStart());
        // The party members that can act this turn
        var activeParty = new List<PartyMember>(Party);
        // Remove party members that are stunned or otherwise don't have a turn
        activeParty.RemoveAll((p) => p.Stunned || !p.HasTurn);
        // Sort the selection order from left to right and top to bottom
        activeParty.Sort((p1, p2) => Pos.CompareLeftToRightTopToBottom(p1.Pos, p2.Pos));
        // Set the cursor's selection list to the party members with turns
        cursor.SetSelected(activeParty);
        cursor.HighlightFirst();
        cursor.SetActive(true);
        return null;
    }

    public override Coroutine OnPhaseEnd()
    {
        cursor.gameObject.SetActive(false);
        // Remove all dead party members
        Party.RemoveAll((obj) => obj == null);
        Party.ForEach((member) => member.OnPhaseEnd());
        return null;
    }

    public void EndAction(PartyMember p)
    {
        // Remove the party member whose action ended
        cursor.RemoveCurrentSelection();
        // Remove any members who were killed as a result of the action or otherwise no longer have a turn
        cursor.RemoveAll((obj) => obj == null || !((obj as PartyMember).HasTurn));
        // If there are no more party members with turns, end the phase
        if (cursor.Empty)
            EndPhase();
        else // Else, highlight the next member of the party
        {
            cursor.HighlightNext();
            cursor.SetActive(true);
        }
    }

    /// <summary>
    /// Cancel the current action (back out of the action menu)
    /// </summary>
    public void CancelAction(PartyMember p)
    {
        cursor.SetActive(true);
    }

    public override void OnPhaseUpdate() { }
}
