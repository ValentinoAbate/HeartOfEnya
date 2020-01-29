using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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
    private readonly List<GameObject> targetGraphics = new List<GameObject>();

    /// <summary>
    /// Base functionality with the addition of start ing by selecting the closes target
    /// Possible improvement: maybe try to select a first target based on a more intelligent metric?
    /// </summary>
    public override void SetActive(bool value)
    {
        base.SetActive(value);
        if (value)
        {
            if (Empty)
                Highlight(inRange.First());
            else
                HighlightFirst();
        }
            
    }

    /// <summary>
    /// Move the cursor ro a new space if the space is legal and in range.
    /// Updates the target pattern display if the cursor is moved, and sets the selection list index if a target is highlighted.
    /// TODO: if a combatant is highlighted, show battle summary?
    /// </summary>
    public override void Highlight(Pos newPos)
    {
        if (!BattleGrid.main.IsLegal(newPos) || !inRange.Contains(newPos))
            return;
        // Update grid and world position
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(newPos);
        // Update and show target pattern
        action.targetPattern.Target(attacker.Pos, newPos);
        action.targetPattern.Show(BattleGrid.main.attackSquareMat);
        // Find highlighted object, and apply applicable extra displays if one exists
        var highlightedObj = BattleGrid.main.GetObject(newPos);
        if (highlightedObj != null)
        {
            int index = SelectionList.IndexOf(highlightedObj);
            if (index != -1)
                selectedInd = index;
        }
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
        SelectionList.Clear();
        inRange.Clear();
        var reachable = BattleGrid.main.Reachable(attacker.Pos, action.range.max, CanMoveThrough);
        foreach(var kvp in reachable)
        {
            if (kvp.Value < action.range.min)
                continue;
            var pos = kvp.Key;
            inRange.Add(pos);
            var obj = BattleGrid.main.GetObject(pos) as Combatant;
            // Put any non-ignored object in the selection list
            if (obj != null && !ignore.Any((t) => t == obj.Team))
                SelectionList.Add(obj);
        }
        SelectionList.Sort((obj1, obj2) => obj1.Team == FieldEntity.Teams.Party ? 1 : -1);
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
            targetGraphics.Add(BattleGrid.main.SpawnSquare(pos, BattleGrid.main.targetSquareMat));
        }
    }

    /// <summary>
    /// Hide any currently displaying target square UI.
    /// Show UI with ShowTargets()
    /// </summary>
    public void HideTargets()
    {
        foreach (var obj in targetGraphics)
            Destroy(obj);
        targetGraphics.Clear();
    }

    /// <summary>
    /// Hide the targeting UI, enact the action, and end the party member's turn.
    /// TODO: integration action display coroutines, move action trigerring to a confirm / deny UI.
    /// </summary>
    public override void Select()
    {
        HideTargets();
        action.targetPattern.Hide();
        StartCoroutine(AttackCr());
        
    }

    private IEnumerator AttackCr()
    {
        enabled = false;
        // Wait for the attack routine to finish
        yield return attacker.UseAction(action, Pos);
        (attacker as PartyMember)?.EndTurn();
        enabled = true;
        // Disable attacking again
        SetActive(false);
    }

    /// <summary>
    /// Basic grid and selection input plus canceling and special input logic for directional attacks.
    /// </summary>
    public override void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Highlight(attacker.Pos);
            HideTargets();
            action.targetPattern.Hide();
            SetActive(false);
            OnCancel.Invoke();
            return;
        }          
        if(action.targetPattern.type == TargetPattern.Type.Spread)
            base.ProcessInput();
        else // Targeting pattern is directional, apply special controls
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Highlight(attacker.Pos + Pos.Up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Highlight(attacker.Pos + Pos.Down);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Highlight(attacker.Pos + Pos.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Highlight(attacker.Pos + Pos.Right);
            }
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

    public bool CanMoveThrough(FieldObject obj)
    {
        return true;
    }
}
