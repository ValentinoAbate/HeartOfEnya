using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Executes in edit mode so that editor positioning utilities can function properly.
/// </summary>
public class IngredientButton : MonoBehaviour
{
    public Ingredient ingredient; //ingredient we're using
    
    private bool selected; //whether or not the button is selected
    private int myID; //what number button we are. Used to help SoupManager keep track of what ingredients are selected

    private void Awake()
    {
    	//update text & images from ingredient
    	UpdateData();
    }

    /// <summary>
    /// Updates images & text in the event that the ingredient changes.
    /// </summary>
    public void UpdateData()
    {
    	//if ingredient isn't set, give an error message & abort
    	if(!ingredient)
    	{
    		Debug.Log("ERROR: No ingredient specified for button " + myID);
    		return;
    	}

    	//set name text based on ingredient
        Text nameTxt = transform.Find("NameText").GetComponent<Text>();
        nameTxt.text = ingredient.name;
        //set effect text based on ingredient
        Text effectTxt = transform.Find("EffectText").GetComponent<Text>();
        effectTxt.text = ingredient.GetEffectText();
        //set ingredient icon based on ingredient
        Image ingIcon = transform.Find("IngredientIcon").GetComponent<Image>();
        ingIcon.sprite = ingredient.ingredientIcon;
    	//set character icon based on ingredient
        Image charIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        charIcon.sprite = ingredient.characterIcon;
    }

    /// <summary>
    /// Sets ID number. Should only be called once during initialization.
    /// </summary>
    public void SetID(int num)
    {
    	myID = num;
    }

    /// <summary>
    /// ToggleSelected will toggle whether the ingredient is selected.
    /// Called when the button is clicked.
    /// </summary>
    public void ToggleSelected()
    {
    	if (selected)
    	{
    		//if already selected, tell the Soup Manager to remove us from the soup
    		SoupManager.main.DisableIngredient(myID);
    		//return to "disabled" color
    		GetComponent<Image>().color = Color.white;
    		//update selected
    		selected = false;
    	}
    	else
    	{
    		//if not already selected, ask the Soup Manager if we can be added to the soup
    		bool added = SoupManager.main.EnableIngredient(myID);
    		
    		if (added)
    		{
    			//if successfully added, update selected & switch to "enabled" color
    			GetComponent<Image>().color = Color.green;
    			selected = true;
    		}
    		else
    		{
    			//we weren't added, probably because soup is full
    			GetComponent<Image>().color = Color.white;
    			selected = false;
    		}
    	}
    }
}
