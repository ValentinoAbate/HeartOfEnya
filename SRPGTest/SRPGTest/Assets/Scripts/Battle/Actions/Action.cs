using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Action : MonoBehaviour
{
    public ActionRange range;
    public TargetPattern targetPattern;
    public int chargeTurns = 0;
    private ActionEffect[] effects;

    private void Awake()
    {
        effects = GetComponentsInChildren<ActionEffect>();
    }

    public void Activate(Combatant user, Pos targetPos)
    {
        targetPattern.Target(user.Pos, targetPos);
        var targetPositions = targetPattern.Positions.ToList();
        // If the targeting pattern is directional, rotate the points to the correct orientation
        if(targetPattern.type == TargetPattern.Type.Directional)
        {
            Pos direction = targetPos - user.Pos;
            Pos DirectionalMode(Pos pos) => Pos.Rotated(user.Pos, pos - direction, Pos.Right, direction);
            targetPositions = targetPositions.Select(DirectionalMode).ToList();
        }
        // Process effects top to bottom, left to right
        targetPositions.Sort((p1, p2) => Pos.CompareTopToBottomLeftToRight(p1, p2));
        foreach (var position in targetPositions)
        {
            var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
            if(target == null)
            {
                continue;
            }
            foreach (var effect in effects)
            {
                ActionEffect.Reaction r = effect.CalculateReaction(target);
                effect.ApplyEffect(user, target, r);
                // If the target died from this effect
                if (target == null)
                    break;
            }
        }
        Destroy(gameObject);
    }
}
