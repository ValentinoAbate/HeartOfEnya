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

    public override IEnumerator ApplyEffect(Combatant user, Combatant target)
    {
        Pos direction = Pos.DirectionBasic(user.Pos, target.Pos);
        if (moveType == Direction.Toward || (moveType == Direction.Hybrid && Pos.Distance(user.Pos, target.Pos) > 1))
            direction *= -1;
        Pos destination = target.Pos + direction; //where are we trying to push the object to?
        FieldObject thingAtDestination = BattleGrid.main.GetObject(destination); //what is in our way?
        Debug.Log("moving: " + thingAtDestination);

        //what do we do with thingAtDestination?
        if (!BattleGrid.main.IsLegal(destination)) //destination is a world border
        {
        	Debug.Log("cannot move into world border, so get smote");
        	//copied from DamageEffect
        	Debug.Log(user.DisplayName + " dealt " + moveDamage + " damage to " + target.DisplayName + " with " + name);
        	bool death = moveDamage >= target.Hp;
        	var src = GetComponent<AudioSource>();
        	target.Damage(moveDamage);
        	src.PlayOneShot(target.damageSfx);
        	yield return new WaitForSeconds(target.damageSfx.length);
        	if (death)
        	{
        		src.PlayOneShot(target.deathSfx);
        		yield return new WaitForSeconds(target.deathSfx.length);
        	}
        }
        else if (thingAtDestination == null) //destination is an empty square
        {
        	Debug.Log("moving into empty square");
        	BattleGrid.main.MoveAndSetWorldPos(target, destination);
        	var src = GetComponent<AudioSource>();
        	src.PlayOneShot(target.moveSfx);
        	yield return new WaitForSeconds(target.moveSfx.length);
        }
        else
        {
        	//for now, just do what we were doing before until I add an isMovable variable to obstacles
        	BattleGrid.main.MoveAndSetWorldPos(target, destination);
        	var src = GetComponent<AudioSource>();
        	src.PlayOneShot(target.moveSfx);
        	yield return new WaitForSeconds(target.moveSfx.length);
        }

        // BattleGrid.main.MoveAndSetWorldPos(target, destination);
        // var src = GetComponent<AudioSource>();
        // src.PlayOneShot(target.moveSfx);
        // yield return new WaitForSeconds(target.moveSfx.length);
    }
}
