using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAutoAct : AIComponent<Enemy>
{
    public bool moved = false;
    public override IEnumerator DoTurn(Enemy obj)
    {
        yield return new WaitWhile(() => obj.PauseHandle.Paused);
        if (!moved && BattleGrid.main.IsEmpty(obj.Pos + Pos.Right))
        {
            BattleGrid.main.MoveAndSetWorldPos(obj, obj.Pos + Pos.Right);
            moved = true;
            yield return new WaitWhile(() => obj.PauseHandle.Paused);
            yield return new WaitForSeconds(0.1f);
        }
        yield return obj.Attack(obj.Pos + Pos.Right);
        yield return new WaitWhile(() => obj.PauseHandle.Paused);
    }
}
