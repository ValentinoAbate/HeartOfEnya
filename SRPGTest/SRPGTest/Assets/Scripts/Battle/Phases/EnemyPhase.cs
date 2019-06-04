using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhase : Phase
{
    public List<Enemy> enemies = new List<Enemy>();
    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        enemies.RemoveAll((e) => e == null);
        Debug.Log("Starting Enemy Phase");
        return StartCoroutine(PlayTurns());
    }

    public override void OnPhaseUpdate()
    {
        PhaseManager.main.NextPhase();
    }

    private IEnumerator PlayTurns()
    {
        foreach (var enemy in enemies)
        {
            enemy.OnPhaseStart();
            yield return enemy.DoTurn();
        }
                 
    }

}
