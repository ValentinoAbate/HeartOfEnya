using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A replacement for the old ingredient button that will (hopefully) support drag-&-drop
/// </summary>
public class DraggableIngredient : MonoBehaviour
{
	public Ingredient ingredient; //ingredient we're using
	public Vector3 startPos; //where we originally spawned, and where to return to if not dropped in the pot
	public Pot pot;

	private bool inSoup = false; //are we in da soup?
	private bool dragging = false; //whether we're currently being dragged
	private BoxCollider2D potCollider; //stores the pot's collider component for quick access. Set automatically in Start()
	private BoxCollider2D myCollider; //stores our collider component for quick access. Set automatically in Start()

    // Start is called before the first frame update
    void Start()
    {
    	//update text & images from ingredient
        UpdateData();

        //drag-&-drop stuff
        startPos = transform.position; //store our initial position
        //get some quick references to the pot & ingredient colliders
        potCollider = pot.GetComponent<BoxCollider2D>();
        if(!potCollider)
        {
        	Debug.LogError("ERROR! Pot has no collider!");
        }
        myCollider = GetComponent<BoxCollider2D>();
        if(!myCollider)
        {
        	Debug.LogError("ERROR! Ingredient has no collider!");
        }
    }

    // Update is called once per frame
    void Update()
    {
    	//handle drag-&-drop
        if (dragging)
        {
        	//because everything's so fucking small I've gotta turn these screenspace coords into worldspace
        	//lest the ingredient ping off into space like a goddamn rocket the second you click it
        	Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            transform.position = new Vector3(point.x, point.y, 0.0f); //make real fucking sure the zero Z value is preserved because I can't trust anyone anymore
        }
    }

    //if clicked, start dragging
    private void OnMouseDown()
    {
    	//when clicked, start dragging the object
    	dragging = true;
    	//if we're in the soup, remove ourselves
    	if(inSoup)
    	{
    		pot.RemoveIngredient(this); //get out of the pot
    		SoupManager.main.DisableIngredient(ingredient); //get out of the soup manager
    		inSoup = false;
    	}
    }

    //if unclicked, stop dragging
    private void OnMouseUp()
    {
    	//when unclicked, stop dragging the object
    	dragging = false;
    	
    	//check if we were dropped in the soup & respond accordingly
    	if (myCollider.bounds.Intersects(potCollider.bounds))
    	{
    		//we were dropped in the soup - do we fit?
    		if (pot.AddIngredient(this))
    		{
    			//b e c o m e   s o u p
    			inSoup = true; //only have to update inSoup - AddIngredient took care of our position for us
    			bool added = SoupManager.main.EnableIngredient(ingredient); //tell the soup manager to add us
    			//Error checking just on the offchance that somehow the ingredient could be added to the pot but not the soup manager.
    			//That should be impossible, but better safe than sorry. 
    			if (added) //success
    			{
    				Debug.Log("Ingredient " + ingredient.name + " successfully added to soup!");
    			}
    			else //failure
    			{
    				Debug.LogWarning("Ingredient " + ingredient.name + " can fit in soup, but wasn't added properly!");
    			}
    		}
    		else
    		{
    			//no more room
    			Debug.Log("Oh no! Pot is full");
    			transform.position = startPos; //go back home and cry because the popular kids rejected us yet again
    		}
    	}
    	else
    	{
    		//we were not dropped in the soup
    		transform.position = startPos; //go back home and cry because you missed the pot
    	}
    }

    //detect mouse over & display the target/effect data
    private void OnMouseEnter()
    {
    	Debug.Log("Moused over " + ingredient.name);
    }

    //detect mouse no longer over & hide the target/effect data
    private void OnMouseExit()
    {
    	Debug.Log("Mouse left " + ingredient.name);
    }

    /// <summary>
    /// Updates images & text in the event that the ingredient changes.
    /// </summary>
    public void UpdateData()
    {
    	/***ADD THE REST OF THIS ONCE THE UI IS ADDED***/

    	//if ingredient isn't set, give an error message & abort
    	// if(!ingredient)
    	// {
    	// 	Debug.Log("ERROR: No ingredient specified for button " + myID);
    	// 	return;
    	// }

    	//set name text based on ingredient
        // Text nameTxt = transform.Find("NameText").GetComponent<Text>();
        // nameTxt.text = ingredient.name;
        //set effect text based on ingredient
        // Text effectTxt = transform.Find("EffectText").GetComponent<Text>();
        // effectTxt.text = ingredient.GetEffectText();
        //set ingredient icon based on ingredient
        SpriteRenderer ingIcon = GetComponent<SpriteRenderer>();
        ingIcon.sprite = ingredient.ingredientIcon;
    	//set character icon based on ingredient
        // Image charIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        // charIcon.sprite = ingredient.characterIcon;
    }
}
