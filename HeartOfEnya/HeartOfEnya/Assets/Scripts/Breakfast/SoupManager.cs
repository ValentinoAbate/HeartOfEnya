using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls all soup-related items & behaviors, including what buttons are active, what ingredients are selected, etc.
/// </summary>
[System.Serializable]
public class SoupManager : MonoBehaviour
{
	//globally-accessible instance used to access the data.
	public static SoupManager main;

	//editor-facing data about the ingredient list
    public int totalIngredients; //how many ingredients we have available
    public int ingredientsPerSoup; //how many ingredients are used per soup
    public List<Ingredient> ingredients = new List<Ingredient>(); //list of all ingredients we have
    public List<bool> enabledIngredients = new List<bool>(); //bools controlling whether a specific ingredient is enabled
    public Ingredient defaultIngredient; //emergency ingredient, used to pad out the list if players collect too few ingredients to make soup
    
    //UI details
    public Transform parentCanvas; //the UI canvas that's the parent of the buttons
    public IngredientButton buttonPrefab; //button prefab used as a template
    public int buttonX; //x-coord of buttons
    public int buttonY; //max y-coord of buttons
    public int buttonOffset; //distance between buttons. Should be >= the size of the button (see the prefab)
    public SoupButton soupButton; //reference to the "make soup" button

    //internal variables used for state tracking, etc.
    private List<int> activeIngredients = new List<int>(); //stores ID numbers of selected ingredients


    /// <summary>
    /// Implements the singleton pattern
    /// </summary>
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Initialize(); //spawn the buttons & perform other startup tasks
        }
        else
            Destroy(gameObject); //there can only be one singleton

    }

    /// <summary>
    /// Performs 1st-time setup, primarily spawning the ingredient buttons & initializing a few variables
    /// </summary>
    private void Initialize()
    {
        //figure out which ingredients are enabled
        List<string> gatheredIngredients = DoNotDestroyOnLoad.Instance.persistentData.gatheredIngredients;
        Debug.Log("***GATHERED " + gatheredIngredients.Count + " INGREDIENTS:***");
        foreach (string x in gatheredIngredients)
        {
            Debug.Log(x);
        }
        Debug.Log("***END GATHERED***");

        //Iterate through the master ingredients list, checking if each one was enabled.
    	//It's an O(n^2) monstrosity, but it's okay because there's only a few ingredients.
        //Plus it helps prevent duplicates by checking each unique ingredient only once.
        int totalSpawned = 0; //how many buttons we've currently spawned
        for (int i = 0; i < totalIngredients; i++)
    	{
            //get an ingredient from the master list, then check if it's present in the gatheredIngredients list
            Ingredient ing = ingredients[i]; //current ingredient from the master list
            if (gatheredIngredients.Contains(ing.name))
            {
                //ingredient was found last round, and should be enabled
                Debug.Log("Enabling ingredient: " + ing.name);
                //gatheredIngredients.Remove(ing.name); //minor optimization; also used to ID any ingredients that don't get found

                //spawn the button
                var button = Instantiate(buttonPrefab);
                //connect it to the UI canvas and move it to the next open slot
                button.transform.SetParent(parentCanvas.transform, false);
                button.transform.localPosition = new Vector3(buttonX, buttonY - (totalSpawned * buttonOffset), 0);
                //set its ingredient & ID
                button.ingredient = ing;
                button.SetID(i); //use i instead of totalSpawned so that we can use the ID as an index to retrieve its ingredient from the ingredients list
                button.UpdateData(); //tell the button to refresh its images with data from the new ingredient

                totalSpawned++;
            }
            else
            {
                //ingredient was not found last round, and thus will not be enabled
                Debug.Log("Ingredient " + ing.name + " isn't enabled");
            }
    	}

        //debug cleanup
        // if (gatheredIngredients.Count > 0)
        // {
        //     Debug.Log("No ingredients found for: " + gatheredIngredients);
        // }

        //sanity check - if fewer than ingredientsPerSoup buttons were spawned, pad out to ingredientsPerSoup ingredients using the default ingredient
        while (totalSpawned < ingredientsPerSoup)
        {
            //spawn the button
            var button = Instantiate(buttonPrefab);
            //connect it to the UI canvas and move it to the next open slot
            button.transform.SetParent(parentCanvas.transform, false);
            button.transform.localPosition = new Vector3(buttonX, buttonY - (totalSpawned * buttonOffset), 0);
            //set its ingredient & ID
            ingredients.Add(defaultIngredient); //work around the ID system by appending copies of the default ingredient to the end of the master list
            button.ingredient = defaultIngredient;
            button.SetID(ingredients.Count - 1);
            button.UpdateData(); //tell the button to refresh its images with data from the new ingredient

            totalSpawned++;
        }
    }

    /// <summary>
    /// Called when the user de-selects an ingredient.
    /// Removes its ID from the list of active ingredients and tells the "make soup" button to update itself
    /// </summary>
    public void DisableIngredient(int ingID)
    {
    	activeIngredients.Remove(ingID);
    	soupButton.UpdateRecipe(activeIngredients);
    }

    /// <summary>
    /// Called when the user selects an ingredient.
    /// If there's still room in the soup, adds its ID from the list of active ingredients, tells the "make soup" button to update itself,
    /// and returns true.
    /// If there's no more room in the soup, returns false.
    /// </summary>
    public bool EnableIngredient(int ingID)
    {
    	if (activeIngredients.Contains(ingID))
    	{
    		//fail if already enabled
    		return false;
    	}
    	else if (activeIngredients.Count >= ingredientsPerSoup)
    	{
    		//fail if we've already filled all ingredient slots
    		return false;
    	}
    	else
    	{
    		//add ingredients to soup and tell the soup button to update its images
    		activeIngredients.Add(ingID);
    		soupButton.UpdateRecipe(activeIngredients);
    		return true; //report success
    	}
    }
}
