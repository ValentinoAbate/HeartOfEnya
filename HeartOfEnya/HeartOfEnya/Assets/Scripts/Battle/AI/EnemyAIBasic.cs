using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAIBasic : AIComponent<Enemy>
{
    public bool stayAway = true;
    public override IEnumerator DoTurn(Enemy self)
    {
        // ref to the action
        var action = self.action;

        #region Lure Code (Deprecated)

        //var lureList = BattleGrid.main.GetAllCombatants((obj) => obj is Lure);   //get list of all lures
        //lureList.RemoveAll((t) => t == null);
        //lureList.Sort((p, p2) => Pos.Distance(self.Pos, p.Pos).CompareTo(Pos.Distance(self.Pos, p2.Pos))); //sort by distance
        //// If there are lures on the field, try and attack them
        //if (lureList.Count > 0)
        //{
        //    var lurePathData = new List<PathData<FieldObject>>();
        //    foreach(var lure in lureList)
        //    {
        //        var path = BattleGrid.main.Path(self.Pos, lure.Pos, (obj) => self.CanMoveThrough(obj) || obj == lure);
        //        if (path == null)
        //            continue;
        //        // Remove the last node (the position of the target)
        //        path.RemoveAt(path.Count - 1);
        //        // Remove the first node (our current position)
        //        path.RemoveAt(0);
        //        lurePathData.Add(new PathData<FieldObject>(lure, path));
        //    }
        //    // No reachable lures, move towards closest lure
        //    if(lurePathData.Count < 1)
        //    {

        //    }
        //}

        #endregion

        // Get the party
        var targetList = new List<Combatant>(PhaseManager.main.PartyPhase.Party);
        // Remove all destroyed targets (just in case)
        targetList.RemoveAll((t) => t == null);
        // Initialize party path
        List<PathData<Combatant>> attackablePathData;
        List<PathData<Combatant>> reachablePathData;
        // Get path data about party members
        GetPathData(self, targetList, out attackablePathData, out reachablePathData);
        // Attack a party member if possible
        if(attackablePathData.Count > 0)
        {
            attackablePathData.Sort((p, p2) => CompareTargetPriority(p.obj, p.path.Count, p2.obj, p2.path.Count));
            yield return StartCoroutine(MoveAndAttackIfAble(self, attackablePathData[0]));
        }
        else if(reachablePathData.Count > 0) // Else more towards a party member
        {
            reachablePathData.Sort((p, p2) => Pos.Distance(p.path[p.path.Count - 1], p.obj.Pos)
                                        .CompareTo(Pos.Distance(p2.path[p2.path.Count - 1], p2.obj.Pos)));
            yield return StartCoroutine(MoveAndAttackIfAble(self, reachablePathData[0]));
        }
        else // Try and fight an obstacle
        {
            // Get all obstacles
            var obstacleList = BattleGrid.main.GetAllCombatants((obj) => obj is Obstacle);   //get list of all obstacles
            obstacleList.RemoveAll((t) => t == null);
            GetPathData(self, obstacleList, out attackablePathData, out reachablePathData);
            if(attackablePathData.Count > 0) // Fight an obstacle if possible
            {
                var partyAvg = Pos.Average(targetList.Select((t) => t.Pos));
                attackablePathData.Sort((p, p2) => Pos.Distance(partyAvg, p.obj.Pos).CompareTo(Pos.Distance(partyAvg, p2.obj.Pos))); //sort by distance
                yield return StartCoroutine(MoveAndAttackIfAble(self, attackablePathData[0]));
            }
            else // Else move towards the party
            {
                var partyAvg = Pos.Average(targetList.Select((t) => t.Pos));
                var reachablePositions = BattleGrid.main.Reachable(self.Pos, self.Move, self.CanMoveThrough).Select((kvp) => kvp.Key).ToList();
                reachablePositions.Sort((p1, p2) => Pos.Distance(partyAvg, p1).CompareTo(Pos.Distance(partyAvg, p2)));
                var path = BattleGrid.main.Path(self.Pos, reachablePositions[0], self.CanMoveThrough);
                yield return StartCoroutine(MoveAlongPath(self, path, path.Count));
            }
        }

        // No target is attackable, move towards closest target
        Debug.Log("No attackable target");
    }

    List<Pos> PathToAttackRange(Enemy self, Combatant target, Action action)
    {
        var potentialPositions = BattleGrid.main.Reachable(self.Pos, self.Move, self.CanMoveThrough);
        var positionTargetDist = potentialPositions.Select((e) => new System.Tuple<Pos, int>(e.Key, Pos.Distance(e.Key, target.Pos)));
        var validPosList = positionTargetDist.Where((t) => action.range.Contains(t.Item2)).ToList();
        if (validPosList.Count == 0)
            return null;
        // Be as close as possible unless we want to stay away
        validPosList.Sort((t1, t2) => stayAway? -t1.Item2.CompareTo(t2.Item2) : t1.Item2.CompareTo(t2.Item2));
        return BattleGrid.main.Path(self.Pos, validPosList[0].Item1, self.CanMoveThrough);
    }

    void GetPathData(Enemy self, List<Combatant> targets, out List<PathData<Combatant>> attackable, out List<PathData<Combatant>> reachable)
    {
        attackable = new List<PathData<Combatant>>();
        reachable = new List<PathData<Combatant>>();
        var action = self.action;
        // Calculate the paths to all party members
        foreach (var target in targets)
        {
            // Find path to target
            List<Pos> path = null;
            if (action.IsRanged)
            {
                path = PathToAttackRange(self, target, action);
                if (path == null) // No attack sqaure, test reachablilty
                {
                    path = BattleGrid.main.Path(self.Pos, target.Pos, (obj) => self.CanMoveThrough(obj) || obj == target);
                    if (path == null)
                        continue;
                    // Log reachable path
                    reachable.Add(new PathData<Combatant>(target, path));
                    continue;
                }
                // Remove the first node (our current position)
                path.RemoveAt(0);
                // Log attacking path
                attackable.Add(new PathData<Combatant>(target, path));
            }
            else
            {
                path = BattleGrid.main.Path(self.Pos, target.Pos, (obj) => self.CanMoveThrough(obj) || obj == target);
                if (path == null)
                    continue;
                // Remove the last node (the position of the target)
                path.RemoveAt(path.Count - 1);
                // Remove the first node (our current position)
                path.RemoveAt(0);
                // Log Reachable path
                reachable.Add(new PathData<Combatant>(target, path));
                // Don't log as attacking if we don't have enough move
                if (path.Count > self.Move + action.range.max - 1)
                    continue;
                // Log attacking path
                attackable.Add(new PathData<Combatant>(target, path));
            }
        }
    }

    IEnumerator MoveAndAttackIfAble(Enemy self, PathData<Combatant> pathData)
    {
        var action = self.action;
        var path = pathData.path;
        if (path.Count > self.Move + action.range.max - 1)
        {
            yield return StartCoroutine(MoveAlongPath(self, path, self.Move));
        }
        else
        {
            yield return StartCoroutine(MoveAlongPath(self, path, path.Count));
            // Actually launch the attack
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            yield return self.Attack(pathData.obj.Pos);
            yield return new WaitWhile(() => self.PauseHandle.Paused);
        }
    }

    // Data on a FieldObject and the Path to it
    public struct PathData<T> where T : FieldObject
    {
        public T obj;
        public List<Pos> path;
        public PathData(T obj, List<Pos> path)
        {
            this.obj = obj;
            this.path = path;
        }
    }

}
