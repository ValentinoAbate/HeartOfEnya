using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds values used for various ingredients.
/// </summary>
[System.Serializable]
public class Ingredient : MonoBehaviour
{
	public enum Effect
    {
        heal,
        restore,
        obscured,
    }

    public enum Character
    {
    	bapy,
    	raina,
    	soleil,
    	lua,
    }

    public string name; //name of ingredient
    public Effect effectType; //whether it affects HP, FP, etc.
    public Character target; //which player if affects. HERE FOR FUTUREPROOFING ONLY.
    public Sprite ingredientIcon; //icon of the ingredient
    public Sprite characterIcon; //icon of the player the ingredient affects
    
    /// <summary>
    /// Converts the value of effectType to a string 
    /// </summary>
    public string GetEffectText()
    {
    	switch (effectType)
    	{
    		case Effect.heal:
    			return "+HP"; //heals user
    		case Effect.restore:
    			return "+FP"; //restores FP
    		case Effect.obscured:
    			return "   "; //special value for if we want to hide the ingredient's effect (e.g. for Lua)
    		default:
    			//shouldn't ever happen but just in case we get a bad effect type
    			Debug.Log("Unknown effect type for ingredient: " + name);
    			return "ERROR";
    	}
    }
}
