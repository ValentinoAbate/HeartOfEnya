using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public override Coroutine DoTurn()
    {
        return StartCoroutine(TurnCR());
    }

    private IEnumerator TurnCR()
    {
        //Debug.Log("FindingReachable");
        //var targetPositions = BattleGrid.main.Reachable(Pos, 1000, ObjType.Enemy, ObjType.Obstacle);
        foreach (var target in PhaseManager.main.Party)
        {
            if (target != null && target.ObjectType != ObjType.Enemy)
            {
                Debug.Log("FindingPath");
                var path = BattleGrid.main.Path(Pos, target.Pos, ObjType.Enemy, ObjType.Obstacle);
                if (path != null)
                {
                    List<GameObject> objs = new List<GameObject>();
                    foreach (var node in path)
                    {
                        objs.Add(BattleGrid.main.SpawnDebugSquare(node));
                        
                    }
                    yield return new WaitForSeconds(2f);
                    foreach (var debugObj in objs)
                        Destroy(debugObj);
                }
                target.Damage(atk);
                Debug.Log(name + " attacks " + target.name + " for " + atk + " damage!");
                yield return new WaitForSeconds(1);
            }
        }
    }
}
