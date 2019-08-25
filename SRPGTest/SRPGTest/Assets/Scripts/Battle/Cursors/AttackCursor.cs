using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackCursor : SelectNextCursor
{
    public FieldObject.Team[] ignore;
    public Combatant attacker;
    private readonly List<GameObject> targetGraphics = new List<GameObject>();
    [System.NonSerialized]
    public ActiveAbility ability;

    public override void SetActive(bool value)
    {
        base.SetActive(value);
        if (value && !Empty)
            HighlightFirst();
        if (value)
            HideTargets(); // TEMP: no structure for cleaning up stuff currently
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
    }

    public void SetAttack(ActiveAbility ability)
    {
        this.ability = ability;
    }

    public void CalculateTargets()
    {
        Pos = attacker.Pos;
        SelectionList.Clear();
        var positions = BattleGrid.main.Reachable(Pos, ability.range.maxRange, CanMoveThrough);
        foreach(var p in positions)
        {
            var obj = BattleGrid.main.GetObject(p) as Combatant;
            if (obj == null || ignore.Any((t) => t == obj.Allegiance))
                continue;
            SelectionList.Add(obj);
        }
    }

    public void ShowTargets()
    {
        Pos = attacker.Pos;
        var positions = BattleGrid.main.Reachable(Pos, ability.range.maxRange, CanMoveThrough);
        foreach (var p in positions)
        {
            var obj = BattleGrid.main.GetObject(p) as Combatant;
            if (obj == null || ignore.Any((t) => t == obj.Allegiance))
                continue;
            targetGraphics.Add(BattleGrid.main.SpawnDebugSquare(p));
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
        var target = Selected as Combatant;
        target.Damage(1);
        SetActive(false);
        (attacker as PartyMember)?.EndAction();
    }

    public bool CanMoveThrough(FieldObject obj)
    {
        return true;
    }
}
