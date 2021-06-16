using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls all soup-related items & behaviors, including what buttons are active, what ingredients are selected, etc.
/// </summary>
[System.Serializable]
public class SoupManager : MonoBehaviour, IPausable
{
    //globally-accessible instance used to access the data.
    public static SoupManager main;

    //editor-facing data about the ingredient list
    public int totalIngredients; //how many ingredients we have available
    public int ingredientsPerSoup; //how many ingredients are used per soup
    public List<Ingredient> ingredients = new List<Ingredient>(); //list of all ingredients we have
    public Ingredient defaultIngredient; //emergency ingredient, used to pad out the list if players collect too few ingredients to make soup
    public Pot pot; //reference to the pot object
    public Camera mainCamera; //reference to the camera. Used to set up the ingredients.

    //UI details
    public Transform parentCanvas; //the UI canvas that's the parent of the buttons
    public DraggableIngredient ingredientPrefab; //draggable ingredient prefab used as a template
    public int ingredientX; //x-coord of buttons
    public int ingredientY; //max y-coord of buttons
    public int ingredientOffset; //distance between buttons. Should be >= the size of the button (see the prefab)
    public SoupButton soupButton; //reference to the "make soup" button

    //internal variables used for state tracking, etc.
    private List<Ingredient> activeIngredients = new List<Ingredient>(); // selected ingredients
    private bool allowInteraction = true; //whether to let the user interact with ingredients, the pot, etc. Only used during the tutorial.
    public bool AllowInteraction //use a property to grant access to allowInteraction and warn of its terrible power
    {
        get { return allowInteraction; }
        set
        {
            allowInteraction = value;
            if (!allowInteraction)
            {
                //since disabling interaction could cause all sorts of fun new errors, throw a warning to make it easier to diagnose "I forgot to turn it back on" errors
                Debug.LogWarning("Disabling Interaction! Remember to turn it back on!");
            }
            else
            {
                Debug.LogWarning("Interaction re-enabled! No problems here!");
            }
        }
    }
    private bool allowOnlyRemove = false; //similar to allowInteraction, but prevents all player actions EXCEPT removing an ingredient from the pot. Only used during the tutorial.
    public bool AllowOnlyRemove //same deal - make sure all use of this potentially destructive property is VERY obvious to prevent terrible new bugs
    {
        get { return allowOnlyRemove; }
        set
        {
            allowOnlyRemove = value;
            if (allowOnlyRemove)
            {
                //since disabling interaction could cause all sorts of fun new errors, throw a warning to make it easier to diagnose "I forgot to turn it back on" errors
                Debug.LogWarning("Disabling All Interactions Except Removal! Remember to turn them back on!");
            }
            else
            {
                Debug.LogWarning("Non-removal blacklist disabled! No problems here!");
            }
        }
    }

    public PauseHandle PauseHandle { get; set; }

    private FMODUnity.StudioEventEmitter sfxCancel;
    private FMODUnity.StudioEventEmitter sfxPlaceItem;

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
        sfxCancel = GameObject.Find("UICancel").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxPlaceItem = GameObject.Find("PlaceItem").GetComponent<FMODUnity.StudioEventEmitter>();
        PauseHandle = new PauseHandle();

        //spawn ingredient buttons
        int totalSpawned = 0; //how many buttons we've currently spawned
        //make sure we don't spawn FP ingredients before they're introduced, or Lua ingredients before she unfreezes
        ingredients = ingredients.FindAll(IsValidIng);
        bool evenMode = ((ingredients.Count % 2) == 0); //whether we align to an even or odd # of objects
        while(totalSpawned < totalIngredients && ingredients.Count > 0)
        {
            //pick a random ingredient that we haven't spawned yet
            int choice = Random.Range(0, ingredients.Count);
            Debug.Log("chosen ingredient " + choice + ", AKA " + ingredients[choice].name);
            
            //spawn the draggable ingredient
            var dragIng = Instantiate(ingredientPrefab);
            dragIng.pot = pot;
            dragIng.transform.localPosition = new Vector3(ingredientX + (ingredientOffset * getOffsetScale(totalSpawned, evenMode)), ingredientY, 0);
            dragIng.ingredient = ingredients[choice]; //set its ingredient
            dragIng.transform.Find("HoverUI").transform.Find("Canvas").GetComponent<Canvas>().worldCamera = mainCamera;
            
            //remove the chosen ingredient from our options & increment totalSpawned
            ingredients.RemoveAt(choice);
            totalSpawned++;
        }

        //sanity check - if fewer than ingredientsPerSoup buttons were spawned, pad out to ingredientsPerSoup ingredients using the default ingredient
        while (totalSpawned < ingredientsPerSoup)
        {
            //spawn the button
            var dragIng = Instantiate(ingredientPrefab);
            dragIng.pot = pot;
            dragIng.transform.localPosition = new Vector3(ingredientX + (ingredientOffset * getOffsetScale(totalSpawned, evenMode)), ingredientY, 0);
            dragIng.ingredient = defaultIngredient; //set its ingredient
            dragIng.transform.Find("HoverUI").transform.Find("Canvas").GetComponent<Canvas>().worldCamera = mainCamera;
            
            totalSpawned++;
        }
    }

    void Start()
    {
        //Invoke the tutorial event. Need to do it here in Start() because otherwise we can't be sure that SoupEvents.main
        //has awoken yet and is non-null (especially since Unity seems determined to initialize us before the event manager) 
        SoupEvents.main.tutorialIntro._event.Invoke();
    }

    /// <summary>
    /// Used to figure out where to place each new ingredient.
    /// Args:
    ///     totalSpawned: how many ingredients have been added so far. Used to index into the right position
    ///     evenMode: whether the first ingredient should be centered in the middle of the screen (for an odd # of ingredients). False = even #; true = odd #
    /// Returns: the value to multiply ingredientOffset by in order to align it properly
    /// </summary>
    private float getOffsetScale(int totalSpawned, bool evenMode)
    {
        float offset;
        if(evenMode) //even #
        {
            offset = 0.5f + (float)Mathf.Floor(totalSpawned/2);
            Debug.Log("EVEN");
        }
        else //odd #
        {
            offset = (float)Mathf.Floor((totalSpawned + 1)/2);
            Debug.Log("ODD");
        }

        //assuming center is 0, try to balance to the left/right of center
        if((totalSpawned % 2) == 0)
        {
            offset *= -1;
        }

        return offset;
    }

    /// <summary>
    /// Called when the user de-selects an ingredient.
    /// Removes its ingredient from the list of active ingredients and tells the "make soup" button to update itself
    /// </summary>
    public void DisableIngredient(Ingredient ing)
    {
        sfxCancel.Play();
        activeIngredients.Remove(ing);
        soupButton.UpdateRecipe(activeIngredients);
    }

    /// <summary>
    /// Called when the user selects an ingredient.
    /// If there's still room in the soup, adds its ingredient to the list of active ingredients, tells the "make soup" button to update itself,
    /// and returns true.
    /// If there's no more room in the soup, returns false.
    /// </summary>
    public bool EnableIngredient(Ingredient ing)
    {
        if (activeIngredients.Contains(ing))
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
            sfxPlaceItem.Play();

            //add ingredients to soup and tell the soup button to update its images
            activeIngredients.Add(ing);
            soupButton.UpdateRecipe(activeIngredients);
            return true; //report success
        }
    }

    /// <summary>
    /// Function that checks whether an ingredient is valid for the current game phase
    /// (i.e. no FP before that mechanic is introduced & no Lua before she's found).
    /// Used in filtering the ingredients prior to spawning them
    /// </summary>
    private static bool IsValidIng(Ingredient ing)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.InTutorialFirstDay || pData.InTutorialSecondDay)
        {
            if (ing.effectType == Ingredient.Effect.restore)
            {
                return false;
            }
        }

        //filter Lua ingredients if she's not free
        if (!pData.LuaUnfrozen)
        {
            if (ing.target == Ingredient.Character.lua)
            {
                return false;
            }
        }

        //if neither of the above is true, the ingredient is valid
        return true;
    }
}
