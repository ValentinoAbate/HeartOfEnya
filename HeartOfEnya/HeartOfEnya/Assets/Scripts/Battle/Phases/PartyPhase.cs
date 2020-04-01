using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; }
    public Cursor Cursor { get => KeyboardMode ? keyboardCursor : mouseCursor as Cursor; }

    public List<PartyMember> Party { get; } = new List<PartyMember>();
    private bool KeyboardMode => keyboardMode;
    [SerializeField]
    private bool keyboardMode = false;

    [SerializeField]
    private SelectionListCursor keyboardCursor;
    [SerializeField]
    private MouseCursor mouseCursor;

    // the party members who still have a turn this phase
    private List<PartyMember> activeParty = new List<PartyMember>();

    private void Awake()
    {
        PauseHandle = new PauseHandle(null, keyboardCursor);
    }

    public override Coroutine OnPhaseStart()
    {
        // Remove all dead and/or gone
        CleanupParty();
        // If no party left, end the battle (just in case)
        if (Party.Count <= 0)
        {
            EndBattle();
            return null;
        }         
        // Call on phse start function for each party member (may need to wait later for DOT effects, stunning, etc.)
        Party.ForEach((p) => p.OnPhaseStart());
        // Reset the active party
        activeParty.Clear();
        activeParty.AddRange(Party);
        // Remove party members that are stunned or otherwise don't have a turn
        activeParty.RemoveAll((p) => p.Stunned || !p.HasTurn);
        // Sort the selection order from left to right and top to bottom
        activeParty.Sort((p1, p2) => Pos.CompareLeftToRightTopToBottom(p1.Pos, p2.Pos));
        // Set the cursor's selection list to the party members with turns (Only if keyboard controls)
        if (KeyboardMode)
        {
            keyboardCursor.SetSelected(activeParty);
            keyboardCursor.HighlightFirst();
            mouseCursor.enabled = false;
        }
        else
            keyboardCursor.enabled = false;
        Cursor.SetActive(true);
        BattleUI.main.ShowEndTurnButton();
        return null;
    }

    public override Coroutine OnPhaseEnd()
    {
        Cursor.SetActive(false);
        BattleUI.main.HideEndTurnButton();
        // Remove all dead and/or gone
        CleanupParty();
        Party.ForEach((member) => member.OnPhaseEnd());
        // If the party is empty, yeet
        if (Party.Count <= 0)
            EndBattle();
        // TODO: visualize end of phase
        return null;
    }

    public void EndAction(PartyMember p)
    {
        // Remove the party member whose action ended
        activeParty.Remove(p);
        // Remove any members who were killed as a result of the action or otherwise no longer have a turn
        activeParty.RemoveAll((obj) => obj == null || !obj.HasTurn);
        // If there are no more party members with turns, end the phase
        if (activeParty.Count <= 0)
        {
            //Hide the info panel;
            BattleUI.main.HideInfoPanel();
            EndPhase();
            return;
        }
        if (KeyboardMode)
        {
            // Remove the party member whose action ended
            keyboardCursor.RemoveCurrentSelection();
            // Remove any members who were killed as a result of the action or otherwise no longer have a turn
            keyboardCursor.RemoveAll((obj) => obj == null || !((obj as PartyMember).HasTurn));
            // Highlight the next member of the party
            keyboardCursor.HighlightNext();
        }
        Cursor.SetActive(true);
        BattleUI.main.ShowEndTurnButton();
    }

    /// <summary>
    /// Cancel the current action (back out of the action menu)
    /// </summary>
    public void CancelAction(PartyMember p)
    {
        Cursor.SetActive(true);
        BattleUI.main.ShowEndTurnButton();
    }

    public override void OnPhaseUpdate() { }

    /// <summary>
    /// Clears dead an otherwise missing (ranaway, etc) members from the party
    /// </summary>
    private void CleanupParty()
    {
        // Remove all dead party members and ranaway members
        Party.RemoveAll((obj) => obj == null || obj.RanAway);
    }
}
