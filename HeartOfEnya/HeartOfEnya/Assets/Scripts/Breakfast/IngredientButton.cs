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
    	//set effect text based on ingredient
        Text effectTxt = transform.Find("EffectText").GetComponent<Text>();
        effectTxt.text = ingredient.GetEffectText();
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
        //Ask the Soup Manager if there's room for us in the soup

        //TEMPORARY CRAP
        Text txt = transform.Find("Text").GetComponent<Text>();
    	txt.text = ingredient.name;
    }
}
