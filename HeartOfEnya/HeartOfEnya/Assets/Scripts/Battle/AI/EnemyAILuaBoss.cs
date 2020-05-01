using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAILuaBoss : AIComponent<Enemy>
{
    public bool allowRepeats = false;
    private static readonly int[] choices = { 0, 1, 2, 3, 4 };
    private int lastRowTargeted = -1;
    public override IEnumerator DoTurn(Enemy self)
    {
        yield return new WaitWhile(() => self.PauseHandle.Paused);
        var modChoices = new List<int>(choices);
        // Removed the last targeted pos if repeats are disabled
        if (!allowRepeats)
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
