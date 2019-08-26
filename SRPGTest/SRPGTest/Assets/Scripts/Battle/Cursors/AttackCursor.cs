using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackCursor : GridAndSelectNextCursor
{
    public FieldObject.Team[] ignore;
    public Combatant attacker;
    private readonly List<GameObject> targetGraphics = new List<GameObject>();
    [System.NonSerialized]
    public ActiveAbility attack;
    public GameObject attackSquarePrefab;
    private readonly HashSet<Pos> inRange = new HashSet<Pos>();

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
        attack.targetPattern.Target(attacker.Pos, newPos);
        attack.targetPattern.Show(attackSquarePrefab);
        var highlightedObj = BattleGrid.main.GetObject(newPos);
        if (highlightedObj != null)
        {
            int index = SelectionList.IndexOf(highlightedObj);
            if (index != -1)
                selectedInd = index;
        }
    }

    public void SetAttack(ActiveAbility ability)
    {
        this.attack = ability;
    }

    public void CalculateTargets()
    {
        SelectionList.Clear();
        inRange.Clear();
        var reachable = BattleGrid.main.Reachable(attacker.Pos, attack.range.maxRange, CanMoveThrough);
        foreach(var kvp in reachable)
        {
            if (kvp.Value < attack.range.minRange)
                continue;
            var pos = kvp.Key;
            inRange.Add(pos);
            var obj = BattleGrid.main.GetObject(pos) as Combatant;
            if (obj != null && !ignore.Any((t) => t == obj.Allegiance))
                SelectionList.Add(obj);
        }
        SelectionList.Sort((obj1, obj2) => obj1.Allegiance == FieldEntity.Team.Party ? 1 : -1);
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
        attack.targetPattern.Hide();
        var atkClone = Instantiate(attack);
        atkClone.Activate(attacker, Pos);
        SetActive(false);
        (attacker as PartyMember)?.EndAction();
        
    }

    public override void ProcessInput()
    {
        if(attack.targetPattern.type == TargetPattern.Type.Spread)
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
