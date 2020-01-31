using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enemy AI that moves one squre to the right (to move out of the spawn zone),
/// and then uses its attack on the square to the right of it.
/// Should be used with charged attacks.
/// </summary>
public class EnemyAutoAct : Enemy
{
    private bool moved = false;
    protected override IEnumerator AICoroutine()
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        if (!moved && BattleGrid.main.IsEmpty(Pos + Pos.Right))
        {
            BattleGrid.main.MoveAndSetWorldPos(this, Pos + Pos.Right);
            moved = true;
            yield return new WaitWhile(() => PauseHandle.Paused);
            yield return new WaitForSeconds(0.1f);
        }
        yield return Attack(Pos + Pos.Right);
        yield return new WaitWhile(() => PauseHandle.Paused);
    }
}
