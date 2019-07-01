using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseWholeTeam : EnemyPhase
{
    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Enemies.RemoveAll((e) => e == null);
        // Farthest to right takes action first
        Enemies.Sort((e, e2) => e2.Col.CompareTo(e.Col));
        return StartCoroutine(PlayTurns());
    }

    public override void OnPhaseUpdate()
    {
        EndPhase();
    }

    private IEnumerator PlayTurns()
    {
        foreach (var enemy in Enemies)
        {
            yield return enemy.StartTurn();
        }
                 
    }

}
