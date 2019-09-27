using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : Enemy
{
    protected override IEnumerator AICoroutine()
    {
        // Sort targets by distance
        var targetList = new List<PartyMember>(PhaseManager.main.PartyPhase.Party);
        targetList.RemoveAll((t) => t == null);
        targetList.Sort((p, p2) => Pos.Distance(Pos, p.Pos).CompareTo(Pos.Distance(Pos, p2.Pos)));

        foreach (var target in targetList)
        {
            // Find path to target
            var path = BattleGrid.main.Path(Pos, target.Pos, (obj) => CanMoveThrough(obj) || obj == target);
            // Mone on to next target if no path is found
            if (path == null)
                continue;
            // Remove the last node (the position of the target)
            path.RemoveAt(path.Count - 1);
            // Remove the first node (our current position)
            path.RemoveAt(0);
            // Move along the path
            for (int i = 0; i < Move && i < path.Count; ++i)
            {
                yield return new WaitWhile(() => PauseHandle.Paused);
                BattleGrid.main.MoveAndSetPosition(this, path[i]);
                yield return new WaitForSeconds(0.1f);
            }
            if (Move >= path.Count)
            {
                yield return new WaitWhile(() => PauseHandle.Paused);
                Attack(target.Pos);
                yield return new WaitForSeconds(1);
            }
            yield return new WaitWhile(() => PauseHandle.Paused);
            break;
        }
    }
}
