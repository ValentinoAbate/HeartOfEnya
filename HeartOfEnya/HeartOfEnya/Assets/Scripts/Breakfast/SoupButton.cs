using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Executes in edit mode so that editor positioning utilities can function properly.
/// </summary>
public class SoupButton : MonoBehaviour
{
    public int recipeSize; //# of ingredients per recipe
    public Sprite emptyIcon; //icon to use for unfilled slot
    public Color disabledColor; //color when not enough ingredients for soup
    public Color enabledColor; //color when have enough ingredients for soup

    private bool enabled; //whether we're active (i.e. have a valid recipe)
    private List<int> ingredientList = new List<int>(); //contains ID numbers of all currently selected ingredients

    private void Awake()
    {
    	//initialize variables
        enabled = false;
        UpdateRecipe(new List<int>());
    }

    /// <summary>
    /// Updates images & enabled status whenever the current recipe changes.
    /// </summary>
    public void UpdateRecipe(List<int> ingredients)
    {
    	//update ingredientList to match the new set of ingredients
    	ingredientList = ingredients;

    	//update the image icons
    	for (int i = 0; i < recipeSize; i++)
    	{
    		//get a reference to the current ingredient icon
    		string iconName = "IngredientIcon" + i;
    		Image ingIcon = transform.Find(iconName).GetComponent<Image>();

    		//update its sprite
    		if (i < ingredientList.Count)
    		{
    			//if there's enough ingredients to fill up to this slot, grab the sprite from the respective ingredient
    			int ingID = ingredientList[i]; //get the target ingredient's index in the Soup Manager's master ingredient list
    			Ingredient tgtIng = SoupManager.main.ingredients[ingID]; //grab a reference to the target ingredient
    			ingIcon.sprite = tgtIng.ingredientIcon; //update the icon with the ingredient's image
    		}
    		else
    		{
    			//if there's no ingredient for this slot, just use the empty icon
    			ingIcon.sprite = emptyIcon;
    		}
    	}

    	//check if we have enough ingredients for a full recipe, & enable/disable ourselves accordingly
    	if (ingredientList.Count >= recipeSize)
    	{
    		//we have enough ingredients - enable the button
    		enabled = true;
    		GetComponent<Image>().color = enabledColor;
    	}
    	else
    	{
    		//we don't have enough ingredients - disable the button
    		enabled = false;
    		GetComponent<Image>().color = disabledColor;
    	}
    }

    /// <summary>
    /// Checks if we have a valid recipe, and if so makes the soup and loads the next level.
    /// Requires a string to tell it which level to load on successful soup-making
    /// </summary>
    public void MakeSoup(string nextSceneName)
    {
    	if (enabled)
    	{
    		//if the recipe is valid, perform any final actions and load the next level
    		/***PERSISTANT DATA STORAGE & OTHER END-OF-SCENE JUNK GOES HERE***/
    		SceneTransitionManager.main.TransitionScenes(nextSceneName);
    	}
    	else
    	{
    		//if the button should do anything special when clicked with an incomplete recipe, put it here
    		Debug.Log("Invalid Recipe - soup failed.");
    	}
    }
}
