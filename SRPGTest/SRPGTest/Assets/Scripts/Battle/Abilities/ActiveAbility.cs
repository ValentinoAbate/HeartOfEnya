using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActiveAbility : MonoBehaviour
{
    public AbilityRange range;
    public TargetPattern targetPattern;
    public float accuracy;
    private ActiveAbilityEffect[] effects;

    private void Awake()
    {
        effects = GetComponentsInChildren<ActiveAbilityEffect>();
    }

    public void Activate(Combatant user, Pos targetPos)
    {
        targetPattern.Target(targetPos);
        var targetPositions = targetPattern.Positions.ToList();
        // Process effects top to bottom, left to right
        targetPositions.Sort((p1, p2) => Pos.CompareTopToBottomLeftToRight(p1, p2));
        foreach (var position in targetPositions)
        {
            var pos = targetPattern.type == TargetPattern.Type.Directional ? 
                RotatedAroundToDirection(user.Pos, targetPos - user.Pos, position) : position;
            var target = BattleGrid.main.GetObject(pos).GetComponent<Combatant>();
            if(target == null)
            {
                continue;
            }
            if (Random.Range(0, 1f) > accuracy)
            {
                Debug.Log("Ability: " + name + " used by " + user.name + " missed target: " + target.name);
                continue;
            }
            foreach (var effect in effects)
            {
                ActiveAbilityEffect.Reaction r = effect.CalculateReaction(target);
                effect.ApplyEffect(user, target, r);
            }
        }
    }

    private Pos RotatedAroundToDirection(Pos center, Pos direction, Pos targetPos)
    {
        Pos difference = center - targetPos;
        if(direction == Pos.Up)
            return center.Offset(-difference.col, difference.row);
        if(direction == Pos.Left)
            return center.Offset(-difference.row, -difference.col);
        if (direction == Pos.Down)
            return center.Offset(difference.col, -difference.row);
        return targetPos; // direction is right or not correct
    }
}
