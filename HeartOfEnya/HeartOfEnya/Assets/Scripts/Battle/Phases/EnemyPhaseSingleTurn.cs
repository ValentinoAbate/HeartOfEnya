using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseSingleTurn : EnemyPhase
{
    public override PauseHandle PauseHandle { get; set; }
    public GameObject nextToGo;
    public Queue<Enemy> EnemiesTurnOrder { get; private set; } = null;
    

    private Enemy activeTurnMember;
    private void Initialize()
    {
        EnemiesTurnOrder = new Queue<Enemy>();
        Enemies.Sort((p, p2) => p.Col.CompareTo(p2.Col));
        foreach (var member in Enemies)
            EnemiesTurnOrder.Enqueue(member);
    }
    public override Coroutine OnPhaseEnd()
    {
        nextToGo.transform.position = BattleGrid.main.GetSpace(EnemiesTurnOrder.Peek().Pos);
        EnemiesTurnOrder.Enqueue(activeTurnMember);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        if (EnemiesTurnOrder == null)
            Initialize();
        activeTurnMember = null;
        while (activeTurnMember == null && EnemiesTurnOrder.Count > 0)
        {
            activeTurnMember = EnemiesTurnOrder.Dequeue();
        }
        return activeTurnMember.StartTurn();
    }

    public override void OnPhaseUpdate()
    {
        EndPhase();
    }

}
