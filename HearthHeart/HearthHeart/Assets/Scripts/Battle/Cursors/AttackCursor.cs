using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class AttackCursor : GridAndSelectNextCursor
{
    public FieldEntity.Teams[] ignore;
    public Combatant attacker;
    private readonly List<GameObject> targetGraphics = new List<GameObject>();
    [System.NonSerialized]
    public Action action;
    public GameObject attackSquarePrefab;
    private readonly HashSet<Pos> inRange = new HashSet<Pos>();
    public UnityEvent OnCancel = new UnityEvent();

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

    public override void Highlight(Pos newPos)
    {
        if (!BattleGrid.main.IsLegal(newPos) || !inRange.Contains(newPos))
            return;
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(newPos);
        action.targetPattern.Target(attacker.Pos, newPos);
        action.targetPattern.Show(attackSquarePrefab);
        var highlightedObj = BattleGrid.main.GetObject(newPos);
        if (highlightedObj != null)
        {
            int index = SelectionList.IndexOf(highlightedObj);
            if (index != -1)
                selectedInd = index;
        }
    }

    public void SetAction(Action action)
    {
        this.action = action;
    }

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
            if (obj != null && !ignore.Any((t) => t == obj.Team))
                SelectionList.Add(obj);
        }
        SelectionList.Sort((obj1, obj2) => obj1.Team == FieldEntity.Teams.Party ? 1 : -1);
    }

    public void ShowTargets()
    {
        CalculateTargets();
        foreach (var pos in inRange)
        {
            targetGraphics.Add(BattleGrid.main.SpawnDebugSquare(pos));
        }
    }

    public void HideTargets()
    {
        foreach (var obj in targetGraphics)
            Destroy(obj);
        targetGraphics.Clear();
    }

    public override void Select()
    {
        HideTargets();
        action.targetPattern.Hide();
        attacker.UseAction(action, Pos);
        SetActive(false);
        (attacker as PartyMember)?.EndAction();
        
    }

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
        else
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
