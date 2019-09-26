using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Enemy : Combatant, IPausable
{
    public PauseHandle PauseHandle { get; set; }
    public override Teams Team => Teams.Enemy;
 
    private List<Pos> traversable;
    private readonly List<GameObject> squares = new List<GameObject>();

    protected override void Initialize()
    {
        base.Initialize();
        PauseHandle = new PauseHandle();
        PhaseManager.main?.EnemyPhase.Enemies.Add(this);
        PhaseManager.main?.EnemyPhase.PauseHandle.Dependents.Add(this);
    }

    private void OnDestroy()
    {
        PhaseManager.main?.EnemyPhase.PauseHandle.Dependents.Remove(this);
    }

    public override void Highlight()
    {
        var traversable = BattleGrid.main.Reachable(Pos, Move, CanMoveThrough).Keys.ToList();
        traversable.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        traversable.Add(Pos);
        foreach (var spot in traversable)
            squares.Add(BattleGrid.main.SpawnSquare(spot, BattleGrid.main.moveSquareMat));
    }

    public override void UnHighlight()
    {
        foreach (var obj in squares)
            Destroy(obj);
        squares.Clear();
    }


}
