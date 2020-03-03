using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Action : MonoBehaviour
{
    public const float targetHighlightSeconds = 0.25f;
    public const float cutInSeconds = 2.25f;

    public ActionRange range;
    public TargetPattern targetPattern;
    public int chargeTurns = 0;

    #region VFX Fields

    public GameObject cutInPrefab = null;
    public GameObject fxPrefab;
    public float delayAtEnd = 0.25f;

    #endregion

    public string Description => description;
    [SerializeField]
    private string description = "description";
    public string DisplayName => displayName;
    [SerializeField]
    private string displayName = "display name";

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
        // Remove all illegal positions from the list
        targetPositions.RemoveAll((p) => !BattleGrid.main.IsLegal(p));

        //Play cut-in if applicable
        if(cutInPrefab != null)
        {
            var cutIn = Instantiate(cutInPrefab);
            yield return new WaitForSeconds(cutInSeconds);
            Destroy(cutIn);
        }

        var tileType = user.Team == FieldEntity.Teams.Enemy ? TileUI.Type.TargetPreviewEnemy
            : TileUI.Type.TargetPreviewParty;
        // Highlight the targeted squares.
        targetPattern.Show(tileType);
        yield return new WaitForSeconds(targetHighlightSeconds);

        var targetPositionBatches = new List<List<Pos>>() { targetPositions };
        bool useBatches = false;

        var customSortingOrder = GetComponent<VfxOrder>();
        if (customSortingOrder == null)
            targetPositions.Sort((p1, p2) => Pos.CompareTopToBottomLeftToRight(p1, p2));
        else
        {
            targetPositionBatches = customSortingOrder.GetPositions(targetPositions);
            useBatches = customSortingOrder.UseBatches;
        }


        foreach(var batch in targetPositionBatches)
        {
            Coroutine routine = null;
            // Iterate through positions and show VFX
            foreach (var position in batch)
            {
                // Check if a target is in this square
                var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
                if (target != null)
                {
                    routine = PlayActionVfx(target.VfxSpawnPoint);
                }
                else
                {
                    routine = PlayActionVfx(BattleGrid.main.GetSpace(position));
                }
                if (!useBatches)
                    yield return routine;
            }
            if (useBatches)
                yield return routine;
        }


        // Wait for the VFX to finish, wait for the highlight time again, then continue
        yield return new WaitForSeconds(targetHighlightSeconds + delayAtEnd);
        targetPattern.Hide();

        // Apply actual effects to targets and display results
        foreach(var position in targetPositions)
        {
            var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
            if (target != null)
            {
                foreach (var effect in effects)
                {
                    if (effect.target != ActionEffect.Target.Other)
                        continue;
                    effect.ApplyEffect(user, target);
                    // If the target died from this effect
                    if (target == null)
                        break;
                }
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                foreach (var effect in effects)
                {
                    if (effect.target != ActionEffect.Target.Tile)
                        continue;
                    effect.ApplyEffect(user, position);
                    // If the target died from this effect
                    if (target == null)
                        break;
                }
            }
            foreach (var effect in effects)
            {
                if (effect.target != ActionEffect.Target.Self)
                    continue;
                if (target != null)
                    effect.ApplyEffect(target, user);
                else
                    effect.ApplyEffect(user, user);
                yield return new WaitForSeconds(0.25f);
                // If the target died from this effect
                if (target == null)
                    break;
            }
        }

        Destroy(gameObject);
    }

    private Coroutine PlayActionVfx(Vector2 position)
    {
        if (fxPrefab != null)
        {
            var fx = Instantiate(fxPrefab, position, Quaternion.identity).GetComponent<ActionVfx>();
            if (fx != null)
                return fx.Play();
            else
                Debug.LogError("fxPrefab: " + fxPrefab.name + " is missing ActionVFX component");
        }
        return null;
    }
}
