using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBasic : AIComponent<Enemy>
{
    public override IEnumerator DoTurn(Enemy self)
    {
        // ref to the action
        var action = self.action;

        
        var obstacleList = new List<FieldObject>(BattleGrid.main.GetAllObjects((obj) => obj is Obstacle));   //get list of all obstacles


        // Get all obstacles
        obstacleList.RemoveAll((t) => t == null);
        obstacleList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos))); //sort by distance

        var lureList = new List<FieldObject>(BattleGrid.main.GetAllObjects((obj) => obj is Lure));   //get list of all lures
        lureList.RemoveAll((t) => t == null);
        lureList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos))); //sort by distance
        // If there are lures on the field, try and attack them
        if (lureList.Count > 0)
        {
            var lurePathData = new List<PathData<FieldObject>>();
            foreach(var lure in lureList)
            {
                var path = BattleGrid.main.Path(self.Pos, lure.Pos, (obj) => self.CanMoveThrough(obj) || obj == lure);
                if (path == null)
                    continue;
                // Remove the last node (the position of the target)
                path.RemoveAt(path.Count - 1);
                // Remove the first node (our current position)
                path.RemoveAt(0);
                lurePathData.Add(new PathData<FieldObject>(lure, path));
            }
            // No reachable lures, move towards closest lure
            if(lurePathData.Count < 1)
            {

            }
        }

        // Get the party
        var targetList = new List<PartyMember>(PhaseManager.main.PartyPhase.Party);
        // Remove all destroyed targets (just in case)
        targetList.RemoveAll((t) => t == null);
        // Sort targets by priority comparison
        targetList.Sort((p, p2) => CompareTargetPriority(self.Pos, p, p2));
        // Determing which targets are attackable, and attack highest priority attackable target found
        foreach (var target in targetList)
        {
            // Find path to target
            var path = BattleGrid.main.Path(self.Pos, target.Pos, (obj) => self.CanMoveThrough(obj) || obj == target);
            Debug.Log("target: " + target.DisplayName + " path: " + path);
            if(action.IsRanged)
            {
                if(path == null)
                {
                    path = PathToAttackRange(self, action);
                    if (path == null)
                        continue; // ranged and unattackable, continue
                }
                else // Remove the last node (the position of the target)
                    path.RemoveAt(path.Count - 1);
                // Remove the first node (our current position)
                path.RemoveAt(0);
            }
            else if (path != null)
            {
                // Remove the last node (the position of the target)
                path.RemoveAt(path.Count - 1);
                // Remove the first node (our current position)
                path.RemoveAt(0);
                // Return if we don't have enough move
                if (path.Count > self.Move + action.range.max - 1)
                    continue;
            }
            else // attack is melee and there is no path, move on to next highest priority target
            {
                continue;
            }

            // Move along the path until within range
            for (int i = 0; i < path.Count - action.range.max + 1; ++i)
            {
                yield return new WaitWhile(() => self.PauseHandle.Paused);
                BattleGrid.main.MoveAndSetWorldPos(self, path[i]);
                yield return new WaitForSeconds(moveDelay);
            }
            // Actually launch the attack
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            yield return self.Attack(target.Pos);
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            break;
        }
        // No target is attackable, move towards closest target
        Debug.Log("No attackable target");
    }

    List<Pos> PathToAttackRange(Enemy self, Action action)
    {
        return null;
    }

    // Data on a FieldObject and the Path to it
    public struct PathData<T> where T : FieldObject
    {
        T obj;
        List<Pos> path;
        public PathData(T obj, List<Pos> path)
        {
            this.obj = obj;
            this.path = path;
        }
    }

}
