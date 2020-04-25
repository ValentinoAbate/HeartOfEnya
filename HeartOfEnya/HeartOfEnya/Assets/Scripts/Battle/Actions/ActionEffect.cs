using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public enum Target
    { 
        Target = 1,
        Tile,
    }
    public const float effectWaitTime = 0.4f;

    public Target target = Target.Target;

    public abstract IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data);

    public virtual IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data) { yield break; }
    //Chained movement effect (accible to all effects)
    protected IEnumerator DoMove(Combatant user, Combatant target, Pos direction, int moveDamage = 2)
    {
        Pos destination = target.Pos + direction; //where are we trying to push the object to?
        Combatant thingAtDestination = BattleGrid.main.GetObject(destination) as Combatant; //what is in our way?
        Debug.Log("moving: " + thingAtDestination);

        //compile a list of all the objects to move
        Stack<MoveData> toMove = new Stack<MoveData>();
        bool keepGoing = true; //loop control variable
        bool dealDamage = false; //whether or not we deal damage from pushing/pulling into immovable object
        Debug.Log("======BEGINNING TARGET COMPILATION======");
        do
        {
            Debug.Log("added " + target.DisplayName + " to move chain");
            toMove.Push(new MoveData(target, destination)); //add current target to the list
            bool isWorldBorder = !BattleGrid.main.IsLegal(destination); //whether the destination spot is a world border
            if (thingAtDestination == null && !isWorldBorder)
            {
                //if pushing into empty space, end search
                keepGoing = false;
            }
            else if (isWorldBorder || !thingAtDestination.isMovable)
            {
                //if pushing into immovable object or world border, end search and tell us to deal damage
                if (isWorldBorder)
                {
                    Debug.Log("ENCOUNTERED OBSTACLE: WorldBorder");
                }
                else
                {
                    Debug.Log("ENCOUNTERED OBSTACLE: ImmovableObject " + thingAtDestination.DisplayName);
                    //if we found an immovable obstacle, we also have to add it to the movement chain to make sure it takes damage
                    toMove.Push(new MoveData(thingAtDestination, destination)); //since the damage code doesn't care about destination, we can just say to put it where it is
                }
                keepGoing = false;
                dealDamage = true;
            }
            else
            {
                //if there's a movable object in our way, iterate the loop
                target = thingAtDestination;
                destination = target.Pos + direction;
                thingAtDestination = BattleGrid.main.GetObject(destination) as Combatant;
            }
        } while (keepGoing);

        //iterate through our movement chain and apply the movement
        Debug.Log("======BEGINNING MOVEMENT EXECUTION======");
        while (toMove.Count != 0)
        {
            MoveData tgtData = toMove.Pop();
            if (dealDamage)
            {
                //copied from DamageEffect
                Debug.Log(user.DisplayName + " dealt " + moveDamage + " damage to " + tgtData.target.DisplayName + " with " + name);
                tgtData.target.Damage(moveDamage);
                yield return new WaitForSeconds(effectWaitTime);
            }
            else
            {
                //move the object to the specified destination
                Debug.Log(user.DisplayName + " moves " + tgtData.target.DisplayName);
                BattleGrid.main.MoveAndSetWorldPos(tgtData.target, tgtData.destination);
            }
        }
        if (!dealDamage)
        {
            var src = GetComponent<AudioSource>();
            src.PlayOneShot(target.moveSfx);
            yield return new WaitForSeconds(target.moveSfx.length);
        }
    }

    public struct ExtraData
    {
        public Pos actionTargetPos;
        public Pos primaryTargetPos;
    }

    //helper class used to store (target, destination) pairs for chained movement
    public class MoveData
    {
        public Combatant target;
        public Pos destination;

        public MoveData(Combatant t, Pos d)
        {
            target = t;
            destination = d;
        }
    }
}
