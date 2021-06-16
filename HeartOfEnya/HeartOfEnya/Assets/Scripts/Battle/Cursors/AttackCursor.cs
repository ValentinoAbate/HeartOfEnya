using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A cursor class responisible for displaying and enacting attack target selection for party members.
/// Has control of battle flow after an attack is selected from the action menu (see ActionMenu.cs and the party member prefabs), and prompts the attacker to use the action on selection.
/// TODO: Add confirm / deny UI, Add combat preview (maybe standardize these systems)
/// Has Grid navigation and selection list navigation.
/// The action being utilized is set befor ethe cursor is enabled in the action menu.
/// </summary>
public class AttackCursor : GridAndSelectionListCursor
{
    public FieldEntity.Teams[] ignore;
    public Combatant attacker;
    public UnityEvent OnCancel = new UnityEvent();

    [System.NonSerialized]
    public Action action;

    private readonly HashSet<Pos> inRange = new HashSet<Pos>();
    private readonly List<TileUI.Entry> tileUIEntries = new List<TileUI.Entry>();

    // Secondary mode fields
    private Pos primaryTarget = Pos.Zero;
    private bool inSecondaryMode = false;

    /// <summary>
    /// Copied from Mouse Cursor
    /// </summary>
    private void FollowMouse(Vector3 mousePos)
    {
        if (PauseHandle.Paused)
            return;
        //convert mouse coords from screenspace to worldspace to BattleGrid coords
        Pos newPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(mousePos));
        Highlight(newPos);
    }

    /// <summary>
    /// Copied From Mouse Cursor
    /// </summary>
    private void HandleSelect()
    {
        if (PauseHandle.Paused)
            return;
        Select();
    }

    /// <summary>
    /// Base functionality with the addition of start ing by selecting the closes target
    /// Possible improvement: maybe try to select a first target based on a more intelligent metric?
    /// </summary>
    public override void SetActive(bool value)
    {
        base.SetActive(value);
        if (value)
        {
            HighlightInitialize();
        }
    }

    private void HighlightInitialize()
    {
        var temp = Pos;
        Pos = Pos.OutOfBounds;
        Highlight(BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        if (Pos == Pos.OutOfBounds)
            Highlight(temp);
        if (Pos == Pos.OutOfBounds)
            Highlight(attacker.Pos + Pos.Left);
        if (Pos == Pos.OutOfBounds)
            Highlight(attacker.Pos + Pos.Left + Pos.Left);
        if (Pos == Pos.OutOfBounds)
            Highlight(attacker.Pos + Pos.Right);
        if (Pos == Pos.OutOfBounds)
            Highlight(attacker.Pos + Pos.Right + Pos.Right);
    }

    /// <summary>
    /// Move the cursor ro a new space if the space is legal and in range.
    /// Updates the target pattern display if the cursor is moved, and sets the selection list index if a target is highlighted.
    /// TODO: if a combatant is highlighted, show battle summary?
    /// </summary>
    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos || !BattleGrid.main.IsLegal(newPos) || !inRange.Contains(newPos))
            return;
        
        sfxHighlight.Play();
        // Update grid and world position
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(newPos);
        // Update and show target pattern
        action.targetPattern.Target(attacker.Pos, newPos);
        action.targetPattern.Show(TileUI.Type.TargetPreviewParty);
    }

    /// <summary>
    /// Set the action (basically the attack) that the cursor is currently using. Set in the action menu  (see the party member prefabs) when an attack is selected)
    /// </summary>
    public void SetAction(Action action)
    {
        this.action = action;
    }

    /// <summary>
    /// Calculate target-able squares, and log the potential targets combatants of the current action in the selection list.
    /// Ignores the targets teams listed in the ignore array.
    /// Targets are sorted with the party last.
    /// </summary>
    public void CalculateTargets()
    {
        var targetRestrictions = BattleUI.main.TargetableTiles;
        var range = inSecondaryMode ? action.secondaryRange : action.range;
        inRange.Clear();
        var reachable = BattleGrid.main.Reachable(attacker.Pos, range.max, CanMoveThrough);
        foreach(var kvp in reachable)
        {
            if (kvp.Value < range.min)
                continue;
            var pos = kvp.Key;
            // Remove non-cardinal squares from cardinal patterns
            if (range.type == ActionRange.Type.Cardinal && attacker.Pos.row != pos.row && attacker.Pos.col != pos.col)
                continue;
            // Apply target restrictions
            if (targetRestrictions.Count > 0 && !targetRestrictions.Contains(kvp.Key))
                continue;
            inRange.Add(pos);
        }
    }

    /// <summary>
    /// Calculate the target squares and show target square UI on each.
    /// Hide UI with HideTargets().
    /// </summary>
    public void ShowTargets()
    {
        CalculateTargets();
        foreach (var pos in inRange)
        {
            tileUIEntries.Add(BattleGrid.main.SpawnTileUI(pos, TileUI.Type.TargetRangeParty));
        }
    }

    /// <summary>
    /// Hide any currently displaying target square UI.
    /// Show UI with ShowTargets()
    /// </summary>
    public void HideTargets()
    {
        foreach (var entry in tileUIEntries)
            BattleGrid.main.RemoveTileUI(entry);
        tileUIEntries.Clear();
    }

    /// <summary>
    /// Hide the targeting UI, enact the action, and end the party member's turn.
    /// TODO: integration action display coroutines, move action trigerring to a confirm / deny UI.
    /// </summary>
    public override void Select()
    {
        if (!inRange.Contains(Pos))
            return;
        BattleUI.main.HidePrompt();
        sfxSelect.Play();
        if (action.useSecondaryRange && !inSecondaryMode)
        {
            HideTargets();
            primaryTarget = Pos;
            inSecondaryMode = true;
            ShowTargets();
        }
        else
        {
            HideTargets();
            BattleUI.main.HideAttackDescriptionPanel();
            action.targetPattern.Hide();
            StartCoroutine(AttackCr());
        }  
    }

    /// <summary>
    /// Cancels action when player right clicks during wheel
    /// </summary>
    public void Cancel()
    {
        sfxCancel.Play();
        if (inSecondaryMode)
        {
            inSecondaryMode = false;
            HideTargets();
            action.targetPattern.Hide();
            ShowTargets();
            HighlightInitialize();
        }
        else
        {          
            Highlight(attacker.Pos);
            HideTargets();
            action.targetPattern.Hide();
            SetActive(false);
            OnCancel.Invoke();
            BattleUI.main.HideAttackDescriptionPanel();
        }
    }

    private IEnumerator AttackCr()
    {
        enabled = false;
        var partyMember = attacker as PartyMember;
        var bonusMove = action.GetComponent<BonusMove>();
        if (partyMember != null && bonusMove != null)
        {
            var moveCuror = partyMember.GetComponent<MoveCursor>();
            moveCuror.SetBonusMode(bonusMove.range);
        }
        if (inSecondaryMode)
        {
            // Wait for the attack routine to finish
            yield return attacker.UseAction(action, Pos, primaryTarget);
            inSecondaryMode = false;
        }
        else
        {
            // Wait for the attack routine to finish
            yield return attacker.UseAction(action, Pos, Pos.OutOfBounds);
        }
        if(partyMember != null)
        {
            if (bonusMove != null )
            {
                var moveCuror = partyMember.GetComponent<MoveCursor>();
                if(moveCuror.BonusMode)
                    moveCuror.SetActive(true);
            }
            else
            {
                partyMember.EndTurn();
            }
        }     
        enabled = true;
        // Disable attacking again
        SetActive(false);
    }

    private Vector3 oldMousePos;

    public override void ProcessInput()
    {
        if (Input.mousePosition != oldMousePos)
        {
            FollowMouse(Input.mousePosition);
            oldMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelect();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    public bool CanMoveThrough(FieldObject obj)
    {
        return true;
    }
}
