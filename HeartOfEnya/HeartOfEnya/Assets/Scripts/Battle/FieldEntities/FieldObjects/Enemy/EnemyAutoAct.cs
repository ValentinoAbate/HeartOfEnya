using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAutoAct : Enemy
{
    private bool moved = false;
    protected override IEnumerator AICoroutine()
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        if (!moved && BattleGrid.main.IsEmpty(Pos + Pos.Right))
        {
            BattleGrid.main.MoveAndSetPosition(this, Pos + Pos.Right);
            moved = true;
            yield return new WaitWhile(() => PauseHandle.Paused);
            yield return new WaitForSeconds(0.1f);
        }
        Attack(Pos + Pos.Right);
        yield return new WaitWhile(() => PauseHandle.Paused);
        yield return new WaitForSeconds(1);
    }
}
