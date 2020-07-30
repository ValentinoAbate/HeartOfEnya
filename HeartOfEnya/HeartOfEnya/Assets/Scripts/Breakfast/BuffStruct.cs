using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data structure used to store buffs in the form of {effect, target}.
/// </summary>
[System.Serializable]
public class BuffStruct
{
	public enum Effect
	{
		heal,
		restore
	}

	public enum Target
	{
		bapy,
		raina,
		soleil,
		lua
	}

	public Effect effectType;
	public Target targetCharacter;

	//default constructor
	public BuffStruct()
	{
		//default to healing bapy cause they probably need it <3
		effectType = Effect.heal;
		targetCharacter = Target.bapy;
	}

	//constructor to initialize from an Ingredient
	public BuffStruct(Ingredient ing)
	{
		//get the effect type
		switch (ing.effectType)
		{
			case Ingredient.Effect.heal:
				effectType = Effect.heal;
				break;
			case Ingredient.Effect.restore:
				effectType = Effect.restore;
				break;
			default:
				//If we encounter an unexpected ingredient effect, instead of failing just default to heal.
				//This is in case we encounter an ingredient that uses the deprecated "obscured" effect,
				//or in case someone adds new effects to Ingredient.cs and forgets to mirror them here.
				Debug.LogWarning("Unknown effect " + ing.GetEffectText() + " assigned to ingredient " + ing.name + "; buff will default to healing");
				effectType = Effect.heal;
				break;
		}

		//get the target
		switch (ing.target)
		{
			case Ingredient.Character.bapy:
				targetCharacter = Target.bapy;
				break;
			case Ingredient.Character.raina:
				targetCharacter = Target.raina;
				break;
			case Ingredient.Character.soleil:
				targetCharacter = Target.soleil;
				break;
			case Ingredient.Character.lua:
				targetCharacter = Target.lua;
				break;
			default:
				//This should never happen unless the Ingredient is royally borked
				Debug.LogError("Unknown character assigned to ingredient " + ing.name + "; cannot convert to buff");
				break;
		}
	}

	//Override the default ToString() method to better represent the BuffStruct
	public override string ToString()
	{
		string buff;
		string tgt;

		switch (effectType)
		{
			case Effect.heal:
				buff = "heal";
				break;
			case Effect.restore:
				buff = "restore";
				break;
			default:
				buff = "ERROR";
				break;
		}

		switch (targetCharacter)
		{
			case Target.bapy:
				tgt = "Bapy";
				break;
			case Target.raina:
				tgt = "Raina";
				break;
			case Target.soleil:
				tgt = "Soleil";
				break;
			case Target.lua:
				tgt = "Lua";
				break;
			default:
				tgt = "ERROR";
				break;
		}

		return "Buff: " + buff + " " + tgt;
	}
}
