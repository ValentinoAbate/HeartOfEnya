using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public enum Attribute
    {
        Physical,
        Magic,
        Fire,
        Ice,
        Support,
    }

    public enum Reaction
    {
        Neutral,
        Immune,
        Vulnerable,
    }

    public Attribute attribute;
    public abstract void ApplyEffect(Combatant user, Combatant target, Reaction reaction);

    public Reaction CalculateReaction(Combatant target)
    {
        if (target.reactions.ContainsKey(attribute))
            return target.reactions[attribute];
        return Reaction.Neutral;
    }

    [System.Serializable] public class ReactionDict : SerializableCollections.SDictionary<Attribute, Reaction> { }
}
