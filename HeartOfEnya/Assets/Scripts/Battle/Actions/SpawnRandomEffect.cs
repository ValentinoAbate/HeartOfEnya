using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomEffect : ActionEffect
{
    [Range(0, 1)]
    public double spawnAnyChance = 1;
    public List<FieldObject> objPrefabs;
    public List<float> objWeights;
    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Debug.Log(user.DisplayName + " can't spawn in an occupied square!");
        yield break;
    }

    public override IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data)
    {
        if (!(RandomU.instance.RandomDouble() < spawnAnyChance))
            yield break;
        var prefab = RandomU.instance.Choice(objPrefabs, objWeights);
        var obj = Instantiate(prefab, BattleGrid.main.GetSpace(target), Quaternion.identity);
        var fObj = obj.GetComponent<FieldObject>();
        BattleGrid.main.SetObject(target, fObj);
        yield return new WaitForSeconds(effectWaitTime);
    }
}
