using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enemy with basic AI
/// Atempts to move adjacent to the closest target and use its attack.
/// Doesn't understand how to use ranged attacks.
/// Doesn't know how to attack obstacles.
/// Attacks the closest target with a path by graph distance not path distance
/// </summary>
public class EnemyBasic : Enemy
{
    protected override IEnumerator AICoroutine()
    {
        // Sort targets by distance
        var targetList = new List<FieldObject>(PhaseManager.main.PartyPhase.Party);
        var lureList = new List<FieldObject>(BattleGrid.main.GetAllObjects((obj) => obj is Lure));   //get list of all lures
        
        // Remove all dead targets (just in case)
        targetList.RemoveAll((t) => t == null);
        // Sort targets by grid distance, closest to farthest
        targetList.Sort((p, p2) => Pos.Distance(Pos, p.Pos).CompareTo(Pos.Distance(Pos, p2.Pos)));
        lureList.RemoveAll((t) => t == null);
        lureList.Sort((p, p2) => Pos.Distance(Pos, p.Pos).CompareTo(Pos.Distance(Pos, p2.Pos))); //sort by distance

        targetList.InsertRange(0, lureList); //prioritize lures by inserting at front of targetList

        foreach (var target in targetList)
        {
            // Find path to target
            var path = BattleGrid.main.Path(Pos, target.Pos, (obj) => CanMoveThrough(obj) || obj == target);
            // Move on to next target if no path is found
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
                BattleGrid.main.MoveAndSetWorldPos(this, path[i]);
                yield return new WaitForSeconds(0.1f);
            }
            if (Move >= path.Count) // Attack if close enough
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
