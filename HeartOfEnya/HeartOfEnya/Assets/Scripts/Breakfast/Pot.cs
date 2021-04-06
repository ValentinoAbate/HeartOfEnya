using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
	[Tooltip("Coords of the ingredients. MUST BE ORDERED LEFT-TO-RIGHT!")]
	public List<Vector3> ingredientPoints = new List<Vector3>(); //list of coords for displaying the ingredients
    // Party Member prefab references for each level
    public List<GameObject> bapyLvl;
    public List<GameObject> soleilLvl;
    public List<GameObject> rainaLvl;
    public List<GameObject> luaLvl;

	private List<DraggableIngredient> chosenIngredients = new List<DraggableIngredient>(); //stores currently-active ingredients

    // Update is called once per frame
    void Update()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && !SoupManager.main.AllowOnlyRemove)
        {
            RemoveMostRecent();
        }
    }

    //attempts to add an ingredient to the pot.
    //if successful, adds it to the pot and returns true
    //if pot is full, does nothing and returns false
    public bool AddIngredient(DraggableIngredient ing)
    {
    	if (chosenIngredients.Count < ingredientPoints.Count) //do we have room for more ingredients?
    	{
            bool added = SoupManager.main.EnableIngredient(ing.ingredient); //tell the soup manager to add us
            if (!added) //error checking - if the soup manager somehow can't add us, abort the add and throw a warning
            {
                Debug.Log("Ingredient " + ing.ingredient.name + " can fit in pot, but can't be added by the manager! Aborting!");
                return false;
            }
            Debug.Log("Ingredient " + ing.ingredient.name + " successfully added to soup!");
            chosenIngredients.Add(ing); //store it in our active ingredient list
    		ing.transform.position = ingredientPoints[chosenIngredients.Count - 1]; //put the ingredient in its place
            
            //update the ingredient's buff UI
            var target = ing.ingredient.target; //get the target character
            int level = DoNotDestroyOnLoad.Instance.persistentData.PartyLevel; //get the character's current level
            PartyMember targetPrefab = getTargetPrefab(target, level).GetComponent<PartyMember>(); //use target & level to retrieve the correct character prefab
            int baseHP = targetPrefab.maxHp; //get the character's base HP
            int baseFP = targetPrefab.maxFp; //get the character's base FP
            Debug.Log("TARGET: " + target + ", LEVEL: " + level + ", HP: " + baseHP + ", FP: " + baseFP);
            if (IngredientsFortarget(target) >= 2) //figure out if there's a second active ingredient with the same target
            {
                //Since 2 buffs are active and there's no duplicate ingredients, we can assume both buffs are active.
                //Therefore, we can tell both ingredients to display both buffs.
                Debug.Log("Multiple ingredients detected for " + target + ", updating all...");
                foreach (DraggableIngredient chosenIng in chosenIngredients)
                {
                    if (chosenIng.ingredient.target == target)
                    {
                        chosenIng.UpdateBuffUI(baseHP,baseFP,true,true);
                    }
                }
            }
            else //we're the only active ingredient for the target, so we only apply our buff
            {
                switch (ing.ingredient.effectType)
                {
                    case Ingredient.Effect.heal:
                        ing.UpdateBuffUI(baseHP,baseFP,true,false);
                        break;
                    case Ingredient.Effect.restore:
                        ing.UpdateBuffUI(baseHP,baseFP,false,true);
                        break;
                    default:
                        Debug.LogWarning("Unknown effect type (" + ing.ingredient.effectType + ") applied to " + target);
                        break;
                }
            }
            
            //attempt to trigger the tutorial event that runs after you add your first ingredient
            SoupEvents.main.buffExplanation._event.Invoke();

            return true;
    	}
    	else
    	{
    		Debug.Log("Pot is full!");
    		return false;
    	}
    }

    /// <summary>
    /// Removes an ingredient from the pot. Will also handle removing it from the soup manager.
    /// </summary>
    public void RemoveIngredient(DraggableIngredient ing)
    {
    	if(chosenIngredients.Contains(ing))
    	{
    		chosenIngredients.Remove(ing); //delete it from our list of active ingredients
    		UpdateIngredientPositions(); //shift the other active ingredients to fill in any holes
    		SoupManager.main.DisableIngredient(ing.ingredient); //delete the ingredient from the soup manager
            //check if any other active ingredients buffed the same character, and if so tell them to disable the buff from the removed ingredient
            DraggableIngredient secondIng = chosenIngredients.Find(x => x.ingredient.target == ing.ingredient.target);
            if (secondIng != null)
            {
                Debug.Log("Second ingredient detected, removing disabled buff...");
                int level = DoNotDestroyOnLoad.Instance.persistentData.PartyLevel; //get the character's current level
                PartyMember targetPrefab = getTargetPrefab(secondIng.ingredient.target, level).GetComponent<PartyMember>(); //use target & level to retrieve the correct character prefab
                int baseHP = targetPrefab.maxHp; //get the character's base HP
                int baseFP = targetPrefab.maxFp; //get the character's base FP
                switch (secondIng.ingredient.effectType)
                {
                    case Ingredient.Effect.heal:
                        secondIng.UpdateBuffUI(baseHP,baseFP,true,false);
                        break;
                    case Ingredient.Effect.restore:
                        secondIng.UpdateBuffUI(baseHP,baseFP,false,true);
                        break;
                    default:
                        Debug.LogWarning("Unknown effect type (" + secondIng.ingredient.effectType + ") applied to " + secondIng.ingredient.target);
                        break;
                }
            }
            //attempt to trigger tutorial progression
            //SoupEvents.main.threeIngredients._event.Invoke();
    	}
    	else
    	{
    		Debug.LogWarning("Cannot remove ingredient! Mayhaps you've been screwed by references yet again?");
    	}
    }

    /// <summary>
    /// A variant of RemoveIngredient that removes the last ingredient added to the pot.
    /// Used with the right-click functionality.
    /// </summary>
    private void RemoveMostRecent()
    {
        //abort if player interaction is disabled
        if (!SoupManager.main.AllowInteraction)
        {
            return;
        }

        if (chosenIngredients.Count > 0) //only proceed if we have active ingredients
        {
        	DraggableIngredient mostRecent = chosenIngredients[chosenIngredients.Count - 1];
        	mostRecent.ResetPosition(); //send the ingredient back to its initial position
            mostRecent.HideBuffUI();
            RemoveIngredient(mostRecent);
        }
    }

    //if an ingredient is removed, shift all remaining ingredients left to fill the empty space
    public void UpdateIngredientPositions()
    {
    	for (int i = 0; i < chosenIngredients.Count; i++)
    	{
    		chosenIngredients[i].transform.position = ingredientPoints[i];
    	}
    }

    //searches through the lists of character prefabs and returns the one matching the given target and level
    private GameObject getTargetPrefab(Ingredient.Character target, int level)
    {
        switch (target)
        {
            case Ingredient.Character.bapy:
                return bapyLvl[level];
            case Ingredient.Character.raina:
                return rainaLvl[level];
            case Ingredient.Character.soleil:
                return soleilLvl[level];
            case Ingredient.Character.lua:
                return luaLvl[level];
            default:
                Debug.LogError("Unable to resolve target: " + target);
                return null;
        }
    }

    //Searches through the list of active ingredients and counts how many have the specified character as their target.
    private int IngredientsFortarget(Ingredient.Character target)
    {
        int count = 0; //# of matching ingredients we've found
        foreach (DraggableIngredient ing in chosenIngredients)
        {
            if (ing.ingredient.target == target)
            {
                count++;
            }
        }
        return count;
    }
}
