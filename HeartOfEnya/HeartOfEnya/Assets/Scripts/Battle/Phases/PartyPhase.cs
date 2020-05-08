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
    [Header("Willow Action Fields")]
    [SerializeField]
    private Action willowStunAction;
    [SerializeField]
    private Willow willow;

    // the party members who still have a turn this phase
    private List<PartyMember> activeParty = new List<PartyMember>();

    private void Awake()
    {
        PauseHandle = new PauseHandle(null, mouseCursor, keyboardCursor);
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

        BattleEvents.main.tutorialIntro._event.Invoke(); // tutorial event at the start of battle
        BattleEvents.main.tutPushing._event.Invoke();    // day 2 event

        // tutorial event at the start of the second turn
        if(PhaseManager.main.Turn == 2)
        {
            BattleEvents.main.tutSoleilChargeReminder._event.Invoke();
        }
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
        else
        {
            // log average positions of each member
            int totalPos = 0;
            foreach(var member in Party)
            {
                totalPos += member.Pos.col;
            }
            DoNotDestroyOnLoad.Instance.playtestLogger.testData.UpdateAvgPos(totalPos, Party.Count);
        }
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
        var ePhase = PhaseManager.main.EnemyPhase;
        var sPhase = PhaseManager.main.SpawnPhase;
        var isTutorial = DoNotDestroyOnLoad.Instance.persistentData.InTutorialFirstDay;
        ePhase.RemoveDead();
        if (ePhase.Enemies.Count <= 0 && !sPhase.HasActiveSpawners && !(isTutorial && PhaseManager.main.Turn <= 2))
        {
            StartCoroutine(DeclareSpawnsThenFinishEndAction());
        }
        else
        {
            FinishEndAction();
        }

    }

    #region Action Soloing Commands

    public void PartyWideSoloAction(string actionID)
    {
        foreach(var partyMember in Party)
        {
            partyMember.ActionMenu.SoloAction(actionID);
        }
    }

    public void PartyWideSoloAction(Action action) => PartyWideSoloAction(action.ID);

    public void PartyWideUnSoloAction(string actionID)
    {
        foreach (var partyMember in Party)
        {
            partyMember.ActionMenu.UnSoloAction(actionID);
        }
    }

    public void PartyWideUnSoloAction(Action action) => PartyWideUnSoloAction(action.ID);

    public void PartyWideClearSoloActions()
    {
        foreach (var partyMember in Party)
        {
            partyMember.ActionMenu.ClearSoloActions();
        }
    }

    #endregion

    public Coroutine WillowStunAll()
    {
        return willow.UseAction(willowStunAction, Pos.Zero, Pos.Zero);
    }

    private IEnumerator DeclareSpawnsThenFinishEndAction()
    {
        yield return PhaseManager.main.SpawnPhase.DeclareNextWave();
        FinishEndAction();
    }

    private void FinishEndAction()
    {
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

        // run tutorial trigger when raina's attack finishes
        BattleEvents.main.tutBapySelect._event.Invoke();

        // tutorial day 2: run triggers after bapy pushes the boxes on each turn
        switch (PhaseManager.main.Turn)
        {
            case 1:
                BattleEvents.main.tutKnockOn._event.Invoke();
                break;
            case 2:
                BattleEvents.main.tutMoveDamage._event.Invoke();
                break;
            case 3:
                BattleEvents.main.tutPulling._event.Invoke();
                break;
            case 4:
                BattleEvents.main.tutChokePoints._event.Invoke();
                break;
        }

        // tutorial day 3: run trigger when raina uses flame cleave
        
    }

    /// <summary>
    /// Cancel the current action (back out of the action menu)
    /// </summary>
    public void CancelAction(PartyMember p)
    {
        Cursor.SetActive(true);
        BattleUI.main.ShowEndTurnButton();

        // tutorial trigger for bapy cancelling action
        if(p.GetName() == "Bapy")
        {
            BattleEvents.main.tutSoleilSelect._event.Invoke();
        }
    }

    public override void OnPhaseUpdate() { }

    /// <summary>
    /// Clears dead an otherwise missing (ranaway, etc) members from the party
    /// </summary>
    private void CleanupParty()
    {
        // Remove all dead party members and runaway members
        Party.RemoveAll((obj) => obj == null || obj.RanAway);
    }

    // turns off ability to select certain units for tutorial scripting
    public void DisableUnits(List<string> units)
    {
        // the fabled n^2. party is never more than 4 anyway so whatever
        foreach(string unit in units)
        {
            foreach(PartyMember member in Party)
            {
                if(member.GetName() == unit)
                {
                    member.DisableUnit();
                }
            }
        }
    }

    // overloaded disable units with only one character input
    public void DisableUnits(string unit)
    {
        foreach(PartyMember member in Party)
        {
            if(member.GetName() == unit)
            {
                member.DisableUnit();
            }
        }
    }

    // the same as disable units but in reverse
    public void EnableUnits(List<string> units)
    {
        foreach(string unit in units)
        {
            foreach(PartyMember member in Party)
            {
                if(member.GetName() == unit)
                {
                    member.EnableUnit();
                }
            }
        }
    }

    // overload with one input
    public void EnableUnits(string unit)
    {
        foreach(PartyMember member in Party)
        {
            if(member.GetName() == unit)
            {
                member.EnableUnit();
            }
        }
    }
}
