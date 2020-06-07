using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAILuaBoss : AIComponent<Enemy>
{
    private static readonly int[] choices = { 0, 1, 2, 3, 4 };
    public override bool StandardActivateChargedAction => false;
    [Tooltip("If true, LuaBoss can target the same row twice in a row")]
    public bool allowRepeatTargetRows = false;
    public bool secondPhase = false;

    public int secondPhaseHp = 6;
    public Action clearObstaclesAndEnemies;
    public Action moveAllToRight;
    public Action clearObstacleRow;
    public Action respawnObstacles;
    public Encounter secondPhaseEnounter;
    public Sprite secondPhaseSprite;

    private int lastRowTargeted = -1;
    public int NumObstaclesDestroyed { get; set; }
    public override IEnumerator DoTurn(Enemy self)
    {
        if(self.IsChargingAction)
        {
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            var target = new Pos(lastRowTargeted, self.Col + 1);
            // Get the number of obstacles in the targeted row
            NumObstaclesDestroyed = BattleGrid.main.FindAll<Obstacle>((c) => c.Row == lastRowTargeted).Count;
            // Clear the obstacles in the row
            yield return self.UseAction(clearObstacleRow, target, target);
            // Activate main move
            yield return self.ActivateChargedAction();
            // Respawn obstacles if any were destroyed
            if (NumObstaclesDestroyed > 0)
                yield return self.UseAction(respawnObstacles, target, target);
            if (!secondPhase)
                yield break;
        }
        yield return new WaitWhile(() => self.PauseHandle.Paused);
        var modChoices = new List<int>(choices);
        // Removed the last targeted pos if repeats are disabled
        if (!allowRepeatTargetRows)
            modChoices.Remove(lastRowTargeted);
        // If the target pattern has reach, we are on the two-row attack, so don't target row 0
        if (self.action.targetPattern.maxReach.y > 0)
            modChoices.Remove(0);
        int targetRow = RandomU.instance.Choice(modChoices);
        yield return self.Attack(new Pos(targetRow, self.Pos.col + 1));
        lastRowTargeted = targetRow;
        yield return new WaitWhile(() => self.PauseHandle.Paused);
    }
}
