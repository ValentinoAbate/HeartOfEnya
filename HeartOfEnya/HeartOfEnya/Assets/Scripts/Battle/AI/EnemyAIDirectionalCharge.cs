using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIDirectionalCharge : AIComponent<Enemy>
{
    public override IEnumerator DoTurn(Enemy self)
    {
        var action = self.action;
        var spacesInRange = BattleGrid.main.Reachable(self.Pos, self.Move, self.CanMoveThrough);
        // Get the party
        var targetList = new List<Combatant>(PhaseManager.main.PartyPhase.Party);
        var targetPositions = targetList.Select((t) => t.Pos);
        // Remove all destroyed targets (just in case)
        targetList.RemoveAll((t) => t == null);
        var partyPosAvg = Pos.Average(targetPositions);
        var directions = Pos.Directions;
        int partyDist = Pos.Distance(self.Pos, partyPosAvg);
        Pos moveTo = self.Pos;
        Pos targetDirection = Pos.Right;
        int bestScore = -1000000;
        foreach(var spaceEntry in spacesInRange)
        {
            var space = spaceEntry.Key;
            int distanceFactor = Pos.Distance(space, partyPosAvg) - partyDist;
            int targetFactor = 0;
            Pos bestDirection = Pos.Right;
            foreach(var direction in directions)
            {
                if (!BattleGrid.main.IsLegal(space + direction))
                    continue;
                var targetedPositions = action.HitPositions(space, space + direction);
                int dirFactor = targetedPositions.Count((p) => targetPositions.Contains(p));
                if(dirFactor > targetFactor)
                {
                    targetFactor = dirFactor;
                    bestDirection = direction;
                }
            }
            int score = distanceFactor + targetFactor * 10;
            if(score > bestScore)
            {
                moveTo = space;
                bestScore = score;
                targetDirection = bestDirection;
            }
        }
        if(moveTo != self.Pos)
        {
            var path = BattleGrid.main.Path(self.Pos, moveTo, self.CanMoveThrough);
            // Move along the path until within range
            for (int i = 0; i < path.Count; ++i)
            {
                yield return new WaitWhile(() => self.PauseHandle.Paused);
                BattleGrid.main.MoveAndSetWorldPos(self, path[i]);
                yield return new WaitForSeconds(moveDelay);
            }
        }
        yield return self.Attack(self.Pos + targetDirection);
    }
}
