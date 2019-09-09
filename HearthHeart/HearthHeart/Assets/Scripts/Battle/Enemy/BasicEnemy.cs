using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BasicEnemy : Enemy
{
    [Header("Enemy-Specific Fields")]
    public Action action;
    public override Coroutine StartTurn()
    {
        return StartCoroutine(TurnCR());
    }

    public override void OnPhaseEnd()
    {
        if(Stunned)
            Stunned = false;
    }

    private IEnumerator TurnCR()
    {
        if(Stunned)
        {
            yield return new WaitForSeconds(1);
            yield break;
        }           
        if(IsChargingAction)
        {
            yield return new WaitForSeconds(1);
            if (ChargingActionReady)
                ActivateChargedAction();
            else
                ChargeChargingAction();
            yield break;
        }
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
            for (int i = 1; i <= Move && i < path.Count; ++i)
            {
                BattleGrid.main.MoveAndSetPosition(this, path[i]);
                yield return new WaitForSeconds(0.1f);
            }
            if(Move + action.range.max >= path.Count)
            {
                if(action.chargeTurns > 0)
                    Debug.Log(name + " begins charging " + action.name);
                else
                    Debug.Log(name + " attacks " + target.name + " with " + action.name);
                Attack(target.Pos);              
                yield return new WaitForSeconds(1);
            }
            
            break;
        }
    }

    private Coroutine Attack(Pos p)
    {
        var target = BattleGrid.main.GetObject(p) as Combatant;
        if(target != null)
        {
            UseAction(action, p);
        }
        return null;
    }
}
