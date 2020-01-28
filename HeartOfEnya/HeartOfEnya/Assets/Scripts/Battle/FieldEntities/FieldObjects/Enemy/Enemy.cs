using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A hostile combabatant (a.k.a an enemy unit).
/// Currently, AI behavior is defined in the AICoroutine which must be defined in a child class.
/// </summary>
public abstract class Enemy : Combatant, IPausable
{
    public PauseHandle PauseHandle { get; set; }
    public override Teams Team => Teams.Enemy;

    [Header("Enemy-Specific Fields")]
    public Action action;

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

    // Show movement range when highlighted.
    // TODO: show attack range, maybe intended action?
    public override void Highlight()
    {
        if (stunned || IsChargingAction)
            return;
        // Calculate and display movement range
        var traversable = BattleGrid.main.Reachable(Pos, Move, CanMoveThrough).Keys.ToList();
        traversable.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        traversable.Add(Pos);
        foreach (var spot in traversable)
            squares.Add(BattleGrid.main.SpawnSquare(spot, BattleGrid.main.moveSquareMat));
    }

    // Hide movement range
    public override void UnHighlight()
    {
        foreach (var obj in squares)
            Destroy(obj);
        squares.Clear();
    }

    /// <summary>
    /// Start and perform this enemy unit's turn.
    /// Actual stuff happens in TurnCR()
    /// </summary>
    public virtual Coroutine StartTurn()
    {
        return StartCoroutine(TurnCR());
    }

    // Unstun if necessary
    public override void OnPhaseEnd()
    {
        if (Stunned)
            Stunned = false;
    }

    /// <summary>
    /// Enemy turn logic, handles stunning and charging if applicable, else allows AI to handle
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnCR()
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        // Apply stun an d exit if stunned
        if (Stunned)
        {          
            yield return new WaitForSeconds(0.25f);
            yield return new WaitWhile(() => PauseHandle.Paused);
            yield break;
        }
        // Else process charge and exit if charging
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
        // Else let the AI coroutine play out the turn
        yield return StartCoroutine(AICoroutine());
    }

    /// <summary>
    /// The AI routine. needs to be overriden in a base class
    /// </summary>
    protected abstract IEnumerator AICoroutine();

    /// <summary>
    /// Attack a position. Basically a wrapper to UseAction, with some addional debugging.
    /// Might be used later for displaying attacks
    /// </summary>
    protected Coroutine Attack(Pos p)
    {
        var target = BattleGrid.main.GetObject(p) as Combatant;
        if (action.chargeTurns > 0)
            Debug.Log(name + " begins charging " + action.name);
        else if (target != null)
            Debug.Log(name + " attacks " + target.name + " with " + action.name);
        return UseAction(action, p);
    }
}
