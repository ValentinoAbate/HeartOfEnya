using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Action : MonoBehaviour
{
    public const float targetHighlightSeconds = 0.25f;
    public const float cutInSeconds = 2f;

    public ActionRange range;
    public TargetPattern targetPattern;
    public int chargeTurns = 0;

    #region VFX Fields

    public GameObject cutInPrefab = null;
    public GameObject fxPrefab;
    public float delayAtEnd = 0.25f;

    #endregion

    private ActionEffect[] effects;

    private void Awake()
    {
        effects = GetComponentsInChildren<ActionEffect>();
    }

    public IEnumerator Activate(Combatant user, Pos targetPos)
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
        // TODO: add custom sorting order.
        targetPositions.Sort((p1, p2) => Pos.CompareTopToBottomLeftToRight(p1, p2));
        var targets = new List<Combatant>();

        //Play cut-in if applicable
        if(cutInPrefab != null)
        {
            var cutIn = Instantiate(cutInPrefab);
            yield return new WaitForSeconds(cutInSeconds);
            Destroy(cutIn);
        }

        // Highlight the targeted squares.
        targetPattern.Show(BattleGrid.main.attackSquareMat);
        yield return new WaitForSeconds(targetHighlightSeconds);

        // Iterate through positions and show VFX
        foreach (var position in targetPositions)
        {
            var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
            if (target != null)
            {
                targets.Add(target);
            }
            if(fxPrefab != null)
            {
                var fx = Instantiate(fxPrefab, BattleGrid.main.GetSpace(position), Quaternion.identity).GetComponent<ActionVfx>();
                if (fx != null)
                    yield return fx.Play();
                else
                    Debug.LogError("fxPrefab: " + fxPrefab.name + " is missing ActionVFX component");
            }
        }

        // Wait for the VFX to finish, wait for the highlight time again, then continue
        yield return new WaitForSeconds(targetHighlightSeconds + delayAtEnd);
        targetPattern.Hide();

        // Apply actual effects to targets and display results
        foreach(var target in targets)
        {
            foreach (var effect in effects)
            {
                ActionEffect.Reaction r = effect.CalculateReaction(target);
                effect.ApplyEffect(user, target, r);
                // If the target died from this effect
                if (target == null)
                    break;
            }
            yield return new WaitForSeconds(0.25f);
        }
        Destroy(gameObject);
    }
}
