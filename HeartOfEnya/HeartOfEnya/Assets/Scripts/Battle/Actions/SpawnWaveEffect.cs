using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaveEffect : ActionEffect
{
    public List<WaveData> waves;
    private Dictionary<Pos, GameObject> spawns;

    public override void Initialize(List<Pos> targetPositions, Combatant user)
    {
        spawns = new Dictionary<Pos, GameObject>();
        var waveData = waves[RandomU.instance.RandomInt(0, waves.Count)];
        foreach(var spawnData in waveData.AllSpawns)
            spawns.Add(spawnData.spawnPosition, spawnData.spawnObject);
    }

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        yield break;
    }

    public override IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data)
    {
        if (!spawns.ContainsKey(target))
            yield break;      
        var obj = Instantiate(spawns[target], BattleGrid.main.GetSpace(target), Quaternion.identity);
        var fObj = obj.GetComponent<FieldObject>();
        BattleGrid.main.SetObject(target, fObj);
        yield return new WaitForSeconds(effectWaitTime);
    }


}
