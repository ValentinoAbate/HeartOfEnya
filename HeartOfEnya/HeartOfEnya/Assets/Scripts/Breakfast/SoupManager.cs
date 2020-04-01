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

    private FMODUnity.StudioEventEmitter sfxSelect;
    private FMODUnity.StudioEventEmitter sfxCancel;

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
    	sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxCancel = GameObject.Find("UICancel").GetComponent<FMODUnity.StudioEventEmitter>();

    	//spawn ingredient buttons
    	int totalSpawned = 0; //how many buttons we've currently spawned
        //create a boolean array to keep track of which ingredients we've already spawned (to prevent duplicates)
    	bool[] enabledIngredients = new bool[ingredients.Count];
    	for (int i = 0; i < 6; i++)
    	{
    		//Random.Range(0, ingredients.Length)
    		//pick a random ingredient that we haven't spawned yet
    		int choice = Random.Range(0, ingredients.Count);
    		while (enabledIngredients[choice])
    		{
    			Debug.Log("already spawned " + ingredients[choice].name + ", skipping");
    			choice = Random.Range(0, ingredients.Count); //re-roll if already enabled

    			//if we have problems with this looping infinitely, add logic to detect if there's no remaining options
    		}
    		enabledIngredients[choice] = true; //set the ingredient to active
    		Debug.Log("chosen ingredient " + choice + ", AKA " + ingredients[choice].name);
    		
    		//spawn the button
    		var button = Instantiate(buttonPrefab);
            //connect it to the UI canvas and move it to the next open slot
            button.transform.SetParent(parentCanvas.transform, false);
            button.transform.localPosition = new Vector3(buttonX, buttonY - (totalSpawned * buttonOffset), 0);
            //set its ingredient & ID
            button.ingredient = ingredients[choice];
            button.SetID(choice); //use i instead of totalSpawned so that we can use the ID as an index to retrieve its ingredient from the ingredients list
            button.UpdateData(); //tell the button to refresh its images with data from the new ingredient

            totalSpawned++;
    	}

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
    	sfxCancel.Play();
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
    		sfxSelect.Play();

            //add ingredients to soup and tell the soup button to update its images
    		activeIngredients.Add(ingID);
    		soupButton.UpdateRecipe(activeIngredients);
    		return true; //report success
    	}
    }
}
