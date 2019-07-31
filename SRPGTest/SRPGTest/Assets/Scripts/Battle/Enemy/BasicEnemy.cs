using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BasicEnemy : Enemy
{
    public override Coroutine StartTurn()
    {
        return StartCoroutine(TurnCR());
    }

    private IEnumerator TurnCR()
    {
        yield return base.StartTurn();
        // Sort targets by distance
        var targetList = new List<PartyMember>(PhaseManager.main.PartyPhase.Party);
        targetList.RemoveAll((t) => t == null);
        targetList.Sort((p, p2) => Pos.Distance(Pos, p.Pos).CompareTo(Pos.Distance(Pos, p2.Pos)));
        
        foreach (var target in targetList)
        {
            var path = BattleGrid.main.Path(Pos, target.Pos, (obj) => CanMoveThrough(obj) || obj == target);
            if (path == null)
                continue;
            path.RemoveAt(path.Count - 1);
            //Skip the first node (our current position)
            for (int i = 1; i <= move && i < path.Count; ++i)
            {
                BattleGrid.main.MoveAndSetPosition(this, path[i]);
                yield return new WaitForSeconds(0.1f);
            }
            if(move >= path.Count - 1)
            {
                //PrepareAction(Attack, new TargetPattern(target.Pos, target.Pos.Offset(0, 1)));
                Attack(target.Pos);
                Debug.Log(name + " attacks " + target.name + " for " + atk + " damage!");
                yield return new WaitForSeconds(1);
            }
            
            break;
        }
    }

    private Coroutine Attack(Pos p)
    {
        var target = BattleGrid.main.GetObject(p) as Combatant;
        target?.Damage(atk);
        return null;
    }
}
