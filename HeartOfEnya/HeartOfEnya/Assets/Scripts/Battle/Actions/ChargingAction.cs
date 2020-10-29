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
    }

    public Type TargetType { get; private set; }
    public bool Ready => true;
    private Action action;
    private TargetPattern displayPattern;
    private TileUI.Type tileType;
    private Pos targetSquare;
    private Pos targetDirection;
    private Combatant user;

    public ChargingAction(Action action, Combatant user, Pos target)
    {
        this.user = user;
        this.action = action;
        if (action.targetPatternGenerator == null)
            displayPattern = action.targetPattern.Clone();
        else
            displayPattern = action.targetPatternGenerator.Generate();
        displayPattern.Target(user.Pos, target);
        tileType = TileUI.Type.ChargingAttackParty;
        if(user.Team == FieldEntity.Teams.Enemy)
        {
            var enemy = user as Enemy;
            tileType = enemy == null || !enemy.isBoss ? TileUI.Type.ChargingAttackEnemy
                : TileUI.Type.ChargingAttackBoss;
        }
        if (action.targetPattern.type == TargetPattern.Type.Spread)
        {
            TargetType = Type.TargetSquare;
            targetSquare = target;
            displayPattern.Show(tileType);
        }
        else // Target pattern is directional
        {
            TargetType = Type.TargetDirection;
            targetDirection = target - user.Pos;
            displayPattern.Show(tileType);
        }
    }

    public void UpdateDisplay()
    {
        UpdateDisplay(user.Pos);
    }

    public void UpdateDisplay(Pos p)
    {
        if (TargetType != Type.TargetDirection)
            return;
        displayPattern.Hide();
        displayPattern.Target(p, p + targetDirection);
        displayPattern.Show(tileType);
    }

    /// <summary>
    /// Activates the charged ability. 
    /// Should only be called if Ready == true or if early activation is intended.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Activate()
    {
        displayPattern.Hide();
        Pos target;
        if (TargetType == Type.TargetSquare)
            target = targetSquare;
        else // Target is direction
            target = user.Pos + targetDirection;
        var actionClone = GameObject.Instantiate(action.gameObject).GetComponent<Action>();
        actionClone.targetPattern = displayPattern;
        return actionClone.Activate(user, target, Pos.OutOfBounds);
    }

    public void Cancel()
    {
        displayPattern.Hide();
    }

}
