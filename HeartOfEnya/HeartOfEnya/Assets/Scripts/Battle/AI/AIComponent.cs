using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIComponent<T> : MonoBehaviour where T : Combatant
{
    // amount of time to pause for each square moved
    public const float moveDelay = 0.1f;
    public abstract IEnumerator DoTurn(T self);

    protected IEnumerator MoveAlongPath(T self, List<Pos> path, int distance)
    {
        // Move along the path until within range
        for (int i = 0; i < distance && i < path.Count; ++i)
        {
            yield return new WaitWhile(() => self.PauseHandle.Paused);
            BattleGrid.main.MoveAndSetWorldPos(self, path[i]);
            yield return new WaitForSeconds(moveDelay);
        }
    }

    public int CompareTargetPriority(Combatant obj1, int pathDist1, Combatant obj2, int pathDist2)
    {
        // First compare grid distance
        int distCmp = pathDist1.CompareTo(pathDist2);
        if (distCmp != 0)
            return distCmp;
        // If grid distance is the same, compare hp
        if (obj1.Hp != obj2.Hp)
            return obj1.Hp.CompareTo(obj2.Hp);
        // Later will compare based on explicit sorting order (names)
        return 0;
    }
}
