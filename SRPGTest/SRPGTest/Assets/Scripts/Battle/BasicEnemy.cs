using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public override Coroutine DoTurn()
    {
        var targetPositions = BattleGrid.main.Reachable(Pos, 1, ObjType.Enemy);
        foreach(var target in targetPositions)
        {
            var obj = BattleGrid.main.GetObject(target) as Combatant;
            if(obj != null && obj.ObjectType != ObjType.Enemy)
            {
                obj.Damage(atk);
                Debug.Log(name + " attacks " + obj.name + " for " + atk + " damage!");
            }
        }
        return null;
    }
}
