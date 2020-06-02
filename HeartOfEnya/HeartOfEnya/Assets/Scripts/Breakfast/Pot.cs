using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Pot : MonoBehaviour
{
	[Tooltip("Coords of the ingredients. MUST BE ORDERED LEFT-TO-RIGHT!")]
	public List<Vector3> ingredientPoints = new List<Vector3>(); //list of coords for displaying the ingredients

	private List<DraggableIngredient> chosenIngredients = new List<DraggableIngredient>(); //stores currently-active ingredients
	private Controls _controls; //used to access the control system for "remove most recent ingredient" functionality

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
    	_controls = new Controls();
    }

    //turn on the "right click to cancel" function
    private void OnEnable()
    {
        _controls.UI.Cancel.performed += RemoveMostRecent; //add RemoveMostRecent to the behaviors performed by left click
        _controls.UI.Cancel.Enable(); //turn on the behavior
    }

    //turn off the "right click to cancel" function
    private void OnDisable()
    {
        _controls.UI.Cancel.performed -= RemoveMostRecent; //remove RemoveMostRecent from the behaviors performed by left
        _controls.UI.Cancel.Disable(); //turn off the behavior
    }

    //attempts to add an ingredient to the pot.
    //if successful, adds it to the pot and returns true
    //if pot is full, does nothing and returns false
    public bool AddIngredient(DraggableIngredient ing)
    {
    	if (chosenIngredients.Count < ingredientPoints.Count) //do we have room for more ingredients?
    	{
    		chosenIngredients.Add(ing); //store it in our active ingredient list
    		ing.transform.position = ingredientPoints[chosenIngredients.Count - 1]; //put the ingredient in its place
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
    private void RemoveMostRecent(InputAction.CallbackContext context)
    {
        if (chosenIngredients.Count > 0) //only proceed if we have active ingredients
        {
        	DraggableIngredient mostRecent = chosenIngredients[chosenIngredients.Count - 1];
        	mostRecent.ResetPosition(); //send the ingredient back to its initial position
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
}
