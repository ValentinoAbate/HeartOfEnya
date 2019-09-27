using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Enemy : Combatant, IPausable
{
    public PauseHandle PauseHandle { get; set; }
    public override Teams Team => Teams.Enemy;

    [Header("Enemy-Specific Fields")]
    public Action action;

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
        if (stunned || IsChargingAction)
            return;
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

    public override Coroutine StartTurn()
    {
        return StartCoroutine(TurnCR());
    }

    public override void OnPhaseEnd()
    {
        if (Stunned)
            Stunned = false;
    }

    private IEnumerator TurnCR()
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        if (Stunned)
        {          
            yield return new WaitForSeconds(1);
            yield return new WaitWhile(() => PauseHandle.Paused);
            yield break;
        }
        if (IsChargingAction)
        {
            yield return new WaitForSeconds(1);
            yield return new WaitWhile(() => PauseHandle.Paused);
            if (ChargingActionReady)
                ActivateChargedAction();
            else
                ChargeChargingAction();
            yield break;
        }
        yield return StartCoroutine(AICoroutine());
    }

    protected abstract IEnumerator AICoroutine();

    protected Coroutine Attack(Pos p)
    {
        var target = BattleGrid.main.GetObject(p) as Combatant;
        if (action.chargeTurns > 0)
            Debug.Log(name + " begins charging " + action.name);
        else if (target != null)
            Debug.Log(name + " attacks " + target.name + " with " + action.name);
        UseAction(action, p);
        return null;
    }
}
