using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIDirectionalCharge : AIComponent<Enemy>
{
    // Minimun is in path distance, maximum is in grid distance
    public ActionRange desiredRange;
    public override IEnumerator DoTurn(Enemy self)
    {
        yield break;
    }
}
