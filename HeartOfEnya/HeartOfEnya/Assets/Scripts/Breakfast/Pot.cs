using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
	[Tooltip("Coords of the ingredients. MUST BE ORDERED LEFT-TO-RIGHT!")]
	public List<Vector3> ingredientPoints = new List<Vector3>(); //list of coords for displaying the ingredients

	private List<DraggableIngredient> chosenIngredients = new List<DraggableIngredient>(); //stores currently-active ingredients

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    //removes an ingredient from the pot
    public void RemoveIngredient(DraggableIngredient ing)
    {
    	if(chosenIngredients.Contains(ing))
    	{
    		chosenIngredients.Remove(ing);
    		UpdateIngredientPositions();
    	}
    	else
    	{
    		Debug.LogWarning("Cannot remove ingredient! Mayhaps you've been screwed by references yet again?");
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
