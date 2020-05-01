using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : ActionEffect
{
    public enum Direction
    {
        Toward,
        Away,
        Hybrid,
    }
    public Direction moveType;
    public int squares = 1;
    public int moveDamage = 2; //how much damage do we take if pushed into world border/immovable object?

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        //quick test - if initial target is immovable, what should we do?
        if(target != null && !target.isMovable)
        {
            // yield break; // uncomment to just abort always
            // Both the target and user are immovable. Break.
            if (!user.isMovable)
            {
                Debug.Log("Target and user are immovable. Aborting.");
                yield break;
            }

        	//If we don't want to just fail, swap things around so that if A tries to push B (who's immobile),
        	//it becomes B pushes A (e.g. if Bapy pushes a pillar, Bapy gets pushed backwards) 
        	Combatant temp = user;
        	user = target;
        	target = temp;
        }

        Pos direction = Pos.DirectionBasic(user.Pos, target.Pos);
        if (moveType == Direction.Toward || (moveType == Direction.Hybrid && Pos.Distance(user.Pos, target.Pos) > 1))
            direction *= -1;
        yield return StartCoroutine(DoMove(user, target, direction, moveDamage));
    }
}


