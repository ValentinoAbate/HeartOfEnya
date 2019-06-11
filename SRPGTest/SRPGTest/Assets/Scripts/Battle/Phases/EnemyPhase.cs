using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhase : Phase
{
    public List<Enemy> Enemies { get; } = new List<Enemy>();

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
        PhaseManager.main.NextPhase();
    }

    private IEnumerator PlayTurns()
    {
        foreach (var enemy in Enemies)
        {
            enemy.OnPhaseStart();
            yield return enemy.DoTurn();
        }
                 
    }

}
