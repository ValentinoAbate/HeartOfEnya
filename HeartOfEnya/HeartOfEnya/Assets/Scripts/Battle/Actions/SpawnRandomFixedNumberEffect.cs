using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomFixedNumberEffect : ActionEffect
{
    public List<FieldObject> objPrefabs;
    public List<float> objWeights;
    public int numberOfSpawns = 3;
    private List<Pos> targetPositions;

    public override void Initialize(List<Pos> positions, Combatant user)
    {
        var luaAI = user.GetComponent<EnemyAILuaBoss>();
        if (luaAI != null)
            numberOfSpawns = luaAI.NumObstaclesDestroyed;
        targetPositions = new List<Pos>(positions);
        targetPositions.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        while(targetPositions.Count > numberOfSpawns)
        {
            targetPositions.RemoveAt(RandomU.instance.RandomInt(0, targetPositions.Count - 1));
        }
    }

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Debug.Log(user.DisplayName + " can't spawn in an occupied square!");
        yield break;
    }

    public override IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data)
    {
        if (!targetPositions.Contains(target))
            yield break;
        var prefab = RandomU.instance.Choice(objPrefabs, objWeights);
        var obj = Instantiate(prefab, BattleGrid.main.GetSpace(target), Quaternion.identity);
        var fObj = obj.GetComponent<FieldObject>();
        BattleGrid.main.SetObject(target, fObj);
        yield return new WaitForSeconds(effectWaitTime);
    }
}
