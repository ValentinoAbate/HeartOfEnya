using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Action : MonoBehaviour
{
    public const float targetHighlightSeconds = 0.25f;
    public const float cutInSeconds = 2.25f;

    public bool IsRanged => range.max > 1;

    public FieldEntity.Teams targetFilter = FieldEntity.Teams.All;
    public ActionRange range;
    public bool useSecondaryRange;
    public ActionRange secondaryRange;
    public TargetPattern targetPattern;
    public TargetPatternGenerator targetPatternGenerator;
    public int chargeTurns = 0;

    #region VFX Fields

    public GameObject cutInPrefab = null;
    public GameObject tileFxPrefab;
    public GameObject actionFxPrefab;
    public GameObject actionFxPrefabVertical;
    public GameObject actionFxPrefabHorizontal;
    public GameObject userFxPrefab;
    public float delayAtEnd = 0.25f;
    #endregion

    public string Description => description;
    [SerializeField]
    private string description = "description";
    public string DisplayName => displayName;
    [SerializeField]
    private string displayName = "display name";
    public string ID => id;
    [SerializeField]
    private string id = "id";

    private ActionEffect[] effects;

    private bool HasPerActionFx
    {
        get
        {
            if (targetPattern.type == TargetPattern.Type.Directional)
                return actionFxPrefabVertical != null && actionFxPrefabHorizontal != null;
            else
                return actionFxPrefab != null;
        }
    }

    private void Awake()
    {
        effects = GetComponentsInChildren<ActionEffect>();
        if (targetPatternGenerator != null)
            targetPattern = targetPatternGenerator.Generate();
    }

    public List<Pos> HitPositions(Pos userPos, Pos targetPos)
    {
        targetPattern.Target(userPos, targetPos);
        var targetPositions = targetPattern.Positions.ToList();
        // If the targeting pattern is directional, rotate the points to the correct orientation
        if (targetPattern.type == TargetPattern.Type.Directional)
        {
            Pos direction = targetPos - userPos;
            Pos DirectionalMode(Pos pos) => Pos.Rotated(userPos, pos - direction, Pos.Right, direction);
            targetPositions = targetPositions.Select(DirectionalMode).ToList();
        }
        // Remove all illegal positions from the list
        targetPositions.RemoveAll((p) => !BattleGrid.main.IsLegal(p));
        return targetPositions;
    }

    public IEnumerator Activate(Combatant user, Pos targetPos, Pos primaryTargetPos)
    {
        // Get the target positions, with rotating applied
        var targetPositions = HitPositions(user.Pos, targetPos);

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

        var fxRoutines = new List<Coroutine>();

        // If there are per-tile fx, play them
        if (tileFxPrefab != null)
            fxRoutines.Add(StartCoroutine(PlayPerTileFx(targetPositions)));
        if (HasPerActionFx)
            fxRoutines.Add(StartCoroutine(PlayPerActionFx(user, targetPos)));
        if (userFxPrefab != null)
            fxRoutines.Add(PlayActionVfx(userFxPrefab,user.VfxSpawnPoint));
        
        // Wait for all fx to finish up
        foreach (var routine in fxRoutines)
            if(routine != null)
                yield return routine;

        // Wait for the VFX to finish, wait for the highlight time again, then continue
        yield return new WaitForSeconds(targetHighlightSeconds + delayAtEnd);
        targetPattern.Hide();
        var extraData = new ActionEffect.ExtraData()
        {
            actionTargetPos = targetPos,
            primaryTargetPos = primaryTargetPos,
        };
        // Initialize the effects
        foreach (var effect in effects)
            effect.Initialize(user);
        List<Combatant> toStun = new List<Combatant>();
        // Apply actual effects to targets and display results
        foreach (var position in targetPositions)
        {
            var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
            if (target != null)
            {
                // Don't hit filtered out targets
                if (!targetFilter.HasFlag(target.Team))
                    continue;
                // Apply effects to targets
                foreach (var effect in effects)
                {
                    if (effect.target != ActionEffect.Target.Target)
                        continue;
                    if (effect is StunEffect)
                    {                     
                        toStun.Add(target);
                        continue;
                    }                       
                    yield return StartCoroutine(effect.ApplyEffect(user, target, extraData));
                    // If the target died from this effect
                    if (target == null)
                        break;
                }
            }
            else
            {
                // Apply effects to tiles
                foreach (var effect in effects)
                {
                    if (effect.target != ActionEffect.Target.Tile)
                        continue;
                    yield return StartCoroutine(effect.ApplyEffect(user, position, extraData));
                    // If the target died from this effect
                    if (target == null)
                        break;
                }
            }
        }
        // Remove all dead combatants
        toStun.RemoveAll((c) => c == null || c.Dead);
        if(toStun.Count > 0)
        {
            yield return new WaitForSeconds(ActionEffect.effectWaitTime);
            foreach (var stunTarget in toStun)
            {
                if (stunTarget.Team == user.Team)
                    continue;
                if (!stunTarget.Stunned)
                    stunTarget.Stunned = true;
            }
            // Play stun sound
            yield return new WaitForSeconds(ActionEffect.effectWaitTime);
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// Play the per-tile fx for this action
    /// </summary>
    /// <param name="targetPositions"></param>
    /// <returns></returns>
    private IEnumerator PlayPerTileFx(List<Pos> targetPositions)
    {
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

        foreach (var batch in targetPositionBatches)
        {
            Coroutine routine = null;
            // Iterate through positions and show VFX
            foreach (var position in batch)
            {
                // Check if a target is in this square
                var target = BattleGrid.main.GetObject(position)?.GetComponent<Combatant>();
                if (target != null)
                {
                    routine = PlayActionVfx(tileFxPrefab, target.VfxSpawnPoint);
                }
                else
                {
                    routine = PlayActionVfx(tileFxPrefab, BattleGrid.main.GetSpace(position));
                }
                if (!useBatches)
                    yield return routine;
            }
            if (useBatches)
                yield return routine;
        }
    }

    private IEnumerator PlayPerActionFx(Combatant user, Pos targetPos)
    {
        if(targetPattern.type == TargetPattern.Type.Directional)
        {
            var direction = Pos.DirectionBasic(user.Pos, targetPos);
            var prefab = actionFxPrefabHorizontal;
            bool flipX = false;
            bool flipY = false;
            if(direction == Pos.Left)
            {
                flipX = true;
            }
            else if(direction == Pos.Up)
            {
                prefab = actionFxPrefabVertical;
            }
            else if(direction == Pos.Down)
            {
                prefab = actionFxPrefabVertical;
                flipY = true;
            }
            flipX = !flipX;
            var actionVfx = prefab.GetComponent<ActionVfx>();
            if (!actionVfx.allowFlipX)
                flipX = false;
            if (!actionVfx.allowFlipY)
                flipY = false;
            yield return PlayActionVfx(prefab, BattleGrid.main.GetSpace(targetPos), flipX, flipY);
        }
        else
        {
            yield return PlayActionVfx(actionFxPrefab, BattleGrid.main.GetSpace(targetPos));
        }
    }

    private Coroutine PlayActionVfx(GameObject prefab, Vector2 position, bool flipX = false, bool flipY = false)
    {
        if (prefab != null)
        {
            var fx = Instantiate(prefab, position, Quaternion.identity).GetComponent<ActionVfx>();
            if (fx != null)
            {
                var scaleVec = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);
                fx.transform.localScale = Vector3.Scale(fx.transform.localScale, scaleVec);
                foreach (Transform t in fx.transform)
                {
                    t.localScale = Vector3.Scale(t.localScale, scaleVec);
                    if(t.childCount > 0)
                    {
                        foreach(Transform t2 in t)
                            t2.localScale = Vector3.Scale(t2.localScale, scaleVec);
                    }
                }
                    
                return fx.Play();
            }
            else
                Debug.LogError("fxPrefab: " + prefab.name + " is missing ActionVFX component");
        }
        return null;
    }
}
