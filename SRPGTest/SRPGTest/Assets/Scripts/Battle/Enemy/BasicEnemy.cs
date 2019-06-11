using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BasicEnemy : Enemy
{
    public override Coroutine DoTurn()
    {
        return StartCoroutine(TurnCR());
    }

    private IEnumerator TurnCR()
    {
        // Sort targets by distance
        var targetList = new List<PartyMember>(PhaseManager.main.PartyPhase.Party);
        targetList.RemoveAll((t) => t == null);
        targetList.Sort((p, p2) => Pos.Distance(Pos, p.Pos).CompareTo(Pos.Distance(Pos, p2.Pos)));
        
        foreach (var target in targetList)
        {
            var path = BattleGrid.main.Path(Pos, target.Pos, ObjType.Enemy, ObjType.Obstacle);
            path.RemoveAt(path.Count - 1);
            if (path == null)
                continue;
            //Skip the first node (our current position)
            for(int i = 1; i <= move && i < path.Count; ++i)
            {
                BattleGrid.main.MoveAndSetPosition(this, path[i]);
                yield return new WaitForSeconds(0.1f);
            }
            if(move >= path.Count - 1)
            {
                target.Damage(atk);
                Debug.Log(name + " attacks " + target.name + " for " + atk + " damage!");
                yield return new WaitForSeconds(1);
            }
            
            break;
        }
    }
}
