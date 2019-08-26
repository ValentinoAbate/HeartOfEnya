using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChargingAction
{
    public enum Type
    {
        TargetDirection,
        TargetSquare,
        TargetObject
    }

    public Type TargetType { get; private set; }
    public bool Ready => TurnsLeft <= 0;
    public int TurnsLeft { get; private set; }
    private Action action;
    private TargetPattern displayPattern;
    private Pos targetSquare;
    private Pos targetDirection;
    private FieldObject targetObj;
    private Combatant user;

    public ChargingAction(Action action, Combatant user, Pos target)
    {
        this.user = user;
        this.action = action;
        displayPattern = action.targetPattern.Clone();
        displayPattern.Target(user.Pos, target);
        TurnsLeft = action.chargeTurns;
        if (action.targetPattern.type == TargetPattern.Type.Spread)
        {
            var obj = BattleGrid.main.GetObject(target);
            if(target == null)
            {
                TargetType = Type.TargetSquare;
                targetSquare = target;
                displayPattern.Show(BattleGrid.main.debugSquarePrefab);
            }
            else
            {
                TargetType = Type.TargetObject;
                targetObj = obj;
                displayPattern.Show(BattleGrid.main.debugSquarePrefab, obj.transform);
            }
        }
        else // Target pattern is directional
        {
            TargetType = Type.TargetDirection;
            targetDirection = target - user.Pos;
            displayPattern.Show(BattleGrid.main.debugSquarePrefab);
        }
    }
    /// <summary>
    /// Charges the ability by one turn.
    /// </summary>
    /// <returns></returns>
    public void Charge() => --TurnsLeft;

    /// <summary>
    /// Activates the charged ability. 
    /// Should only be called if Ready == true or if early activation is intended.
    /// </summary>
    /// <returns></returns>
    public void Activate()
    {
        displayPattern.Hide();
        Pos target;
        if (TargetType == Type.TargetSquare)
            target = targetSquare;
        else if (TargetType == Type.TargetObject)
            target = targetObj.Pos;
        else // Target is direction
            target = user.Pos + targetDirection;
        var actionClone = GameObject.Instantiate(action.gameObject).GetComponent<Action>();
        actionClone.Activate(user, target);
    }

    public void Cancel()
    {
        displayPattern.Hide();
    }

}
