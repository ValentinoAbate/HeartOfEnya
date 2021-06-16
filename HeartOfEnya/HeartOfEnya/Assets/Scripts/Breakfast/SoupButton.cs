using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// The button used to confirm the selected soup recipe & exit the breakfast scene.
/// </summary>
public class SoupButton : MonoBehaviour
{
    public Sprite emptyIcon; //icon to use for unfilled slot
    public Color disabledColor; //color when not enough ingredients for soup
    public Color enabledColor; //color when have enough ingredients for soup
    public Text ingredientCountText; //text object for the "#/3 ingredients" text

    private bool clickEnabled = false; //whether we're active (i.e. have a valid recipe)
    private List<Ingredient> ingredientList = new List<Ingredient>(); //contains all currently selected ingredients

    public EventTrigger trigger;
    private FMODUnity.StudioEventEmitter sfxSelect;
    private FMODUnity.StudioEventEmitter sfxHighlight;
    private FMODUnity.StudioEventEmitter sfxCancel;

    private void Start()
    {
    	//initialize variables
        clickEnabled = false;
        UpdateRecipe(new List<Ingredient>()); //pass UpdateRecipe an empty list to set all icons to the "empty" image

        sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxHighlight = GameObject.Find("UIHighlight").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxCancel = GameObject.Find("UICancel").GetComponent<FMODUnity.StudioEventEmitter>();

        AddEntry(EventTriggerType.PointerEnter, OnSelect);
    }

    /// <summary>
    /// Updates images & enabled status whenever the current recipe changes.
    /// </summary>
    public void UpdateRecipe(List<Ingredient> ingredients)
    {
    	//update ingredientList to match the new set of ingredients
    	ingredientList = ingredients;

    	//update the image icons
    	for (int i = 0; i < SoupManager.main.ingredientsPerSoup; i++)
    	{
    		//get a reference to the current ingredient icon
    		string iconName = "IngredientIcon" + i;
    		Image ingIcon = transform.Find(iconName).GetComponent<Image>();

    		//update its sprite
    		if (i < ingredientList.Count)
    		{
    			//if there's enough ingredients to fill up to this slot, grab the sprite from the respective ingredient
    			ingIcon.sprite = ingredientList[i].ingredientIcon;
    		}
    		else
    		{
    			//if there's no ingredient for this slot, just use the empty icon
    			ingIcon.sprite = emptyIcon;
    		}
    	}

    	//update the ingredient count text
    	ingredientCountText.text = "Selected Ingredients: " + ingredientList.Count + "/" + SoupManager.main.ingredientsPerSoup;

    	//check if we have enough ingredients for a full recipe, & enable/disable ourselves accordingly
    	if (ingredientList.Count >= SoupManager.main.ingredientsPerSoup)
    	{
    		//we have enough ingredients - enable the button
    		clickEnabled = true;
    		GetComponent<Image>().color = enabledColor;
    	}
    	else
    	{
    		//we don't have enough ingredients - disable the button
    		clickEnabled = false;
    		GetComponent<Image>().color = disabledColor;
    	}
    }

    /// <summary>
    /// Checks if we have a valid recipe, and if so makes the soup and loads the next level.
    /// Requires a string to tell it which level to load on successful soup-making
    /// </summary>
    public void MakeSoup(string nextSceneName)
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        if (clickEnabled)
        {
            //if the recipe is valid, perform any final actions and load the next level
            /***PERSISTANT DATA STORAGE & OTHER END-OF-SCENE JUNK GOES HERE***/

            //convert the ingredients into buff structures & write them to persistent data
            List<BuffStruct> buffs = new List<BuffStruct>(); //stores the buff structures used in persistent data
            for (int i = 0; i < ingredientList.Count; i++)
            {
                Ingredient tgtIng = ingredientList[i]; //grab a reference to the target ingredient
                if (tgtIng.name == SoupManager.main.defaultIngredient.name)
                {
                    Debug.Log("Skipping BuffStruct formation for default ingredient");
                }
                else
                {
                    buffs.Add(new BuffStruct(tgtIng)); //convert the ingredient to a BuffStruct & add it to the buff list
                }
            }
            DoNotDestroyOnLoad.Instance.persistentData.buffStructures = buffs; //write to persistent data

            if(sfxSelect != null)
                sfxSelect.Play();

            //load the next level
            DoNotDestroyOnLoad.Instance.persistentData.returnScene = nextSceneName; //mark what scene to return to on game load
            DoNotDestroyOnLoad.Instance.persistentData.SaveToFile(); //autosave
            SceneTransitionManager.main.TransitionScenes(nextSceneName);
        }
        else
        {
            //if the button should do anything special when clicked with an incomplete recipe, put it here
            sfxCancel.Play();
            Debug.Log("Invalid Recipe - soup failed.");
        }
    }

    private void AddEntry(EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        sfxHighlight.Play();
        eventData.Use();
    }
}
