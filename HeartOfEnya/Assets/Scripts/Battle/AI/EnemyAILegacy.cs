using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAILegacy : AIComponent<Enemy>
{
    public override IEnumerator DoTurn(Enemy self)
    {
        // Sort targets by distance
        var targetList = new List<FieldObject>(PhaseManager.main.PartyPhase.Party);
        var lureList = new List<FieldObject>(BattleGrid.main.GetAllObjects((obj) => obj is Lure));   //get list of all lures
        var obstacleList = new List<FieldObject>(BattleGrid.main.GetAllObjects((obj) => obj is Obstacle));   //get list of all obstacles

        // Remove all dead targets (just in case)
        targetList.RemoveAll((t) => t == null);
        // Sort targets by grid distance, closest to farthest
        targetList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos)));
        lureList.RemoveAll((t) => t == null);
        lureList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos))); //sort by distance
        obstacleList.RemoveAll((t) => t == null);
        obstacleList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos))); //sort by distance

        targetList.InsertRange(0, lureList); //prioritize lures by inserting at front of targetList
        targetList.AddRange(obstacleList); //de-prioritize obstacles by inserting at end of targetList

        foreach (var target in targetList)
        {
            // Find path to target
            var path = BattleGrid.main.Path(self.Pos, target.Pos, (obj) => self.CanMoveThrough(obj) || obj == target);
            // Move on to next target if no path is found
            if (path == null)
                continue;
            // Remove the last node (the position of the target)
            path.RemoveAt(path.Count - 1);
            // Remove the first node (our current position)
            path.RemoveAt(0);
            // Move along the path
            for (int i = 0; i < self.Move && i < path.Count; ++i)
            {
                yield return new WaitWhile(() => self.PauseHandle.Paused);
                BattleGrid.main.MoveAndSetWorldPos(self, path[i]);               
                yield return new WaitForSeconds(0.1f);
            }
            if (self.Move >= path.Count) // Attack if close enough
            {
                yield return new WaitWhile(() => self.PauseHandle.Paused);
                yield return self.Attack(target.Pos);
                //yield return new WaitForSeconds(1);
            }
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            break;
        }
    }
}
