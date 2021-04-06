using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A replacement for the old ingredient button that will (hopefully) support drag-&-drop
/// </summary>
public class DraggableIngredient : MonoBehaviour
{
	public Ingredient ingredient; //ingredient we're using
	public Vector3 startPos; //where we originally spawned, and where to return to if not dropped in the pot
	public Pot pot;
	public Vector3 infoCanvasOffset; //offset for the UI canvas
	public float infoCanvasScale; //controls the size of the UI canvas

	private bool inSoup = false; //are we in da soup?
	private bool dragging = false; //whether we're currently being dragged
	private BoxCollider2D potCollider; //stores a reference to the pot's collider component for quick access. Set automatically in Start()
	private BoxCollider2D myCollider; //stores a reference to our collider component for quick access. Set automatically in Start()
	private GameObject hoverUI; //stores a reference to the group for the hover-over UI for quick access. Set automatically in Start()
    private GameObject buffUI; //stores a reference to the group for the in-soup buff UI for quick access. Set automatically in Start()
	private Canvas myCanvas; //stores a reference to the child canvas object for quick access. Set automatically in Start()
    private Canvas myBuffCanvas; //stores a reference to the child buff canvas object for quick access. Set automatically in Start()

    private FMODUnity.StudioEventEmitter sfxHighlight;
    private FMODUnity.StudioEventEmitter sfxCancel;
    private FMODUnity.StudioEventEmitter sfxCabbage;
    private FMODUnity.StudioEventEmitter sfxCarrots;
    private FMODUnity.StudioEventEmitter sfxChicken;
    private FMODUnity.StudioEventEmitter sfxMushroom;
    private FMODUnity.StudioEventEmitter sfxNoodles;
    private FMODUnity.StudioEventEmitter sfxOnions;
    private FMODUnity.StudioEventEmitter sfxPotatoes;
    private FMODUnity.StudioEventEmitter sfxTomatoes;
    private FMODUnity.StudioEventEmitter sfxWalnuts;

    // Start is called before the first frame update
    void Start()
    {
    	//set up some references we'll need later
    	hoverUI = transform.Find("HoverUI").gameObject; //store a reference for later use so we don't have a fuckton of Find() calls
        buffUI = transform.Find("BuffUI").gameObject; //same deal
        myCanvas = transform.Find("HoverUI").transform.Find("Canvas").GetComponent<Canvas>(); //store this bastard, we're gonna use it a lot
        myBuffCanvas = transform.Find("BuffUI").transform.Find("Canvas").GetComponent<Canvas>(); //also store this bastard
        
        sfxHighlight = GameObject.Find("UIHighlight").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxCancel = GameObject.Find("UICancel").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxCabbage = GameObject.Find("SFXCabbage").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxCarrots = GameObject.Find("SFXCarrots").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxChicken = GameObject.Find("SFXChicken").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxMushroom = GameObject.Find("SFXMushroom").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxNoodles = GameObject.Find("SFXNoodles").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxOnions = GameObject.Find("SFXOnions").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxPotatoes = GameObject.Find("SFXPotatoes").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxTomatoes = GameObject.Find("SFXTomatoes").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxWalnuts = GameObject.Find("SFXWalnuts").GetComponent<FMODUnity.StudioEventEmitter>();

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

        //turn off the UI on startup so it only appears when moused over
        hoverUI.SetActive(false);
        buffUI.SetActive(false);

        //For reasons no sane person can explain, the only way to get the fucking UI to show up is if we switch the UI Canvas
        //from "WorldSpace" render mode to "SceenSpace - Camera" and then switch it back.
        //This is probably the single dumbest thing I have ever done in my life, and that counts all the times I intentionally electrocuted myself
        //just to see if my braces were conductive.
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.renderMode = RenderMode.WorldSpace;
        myBuffCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myBuffCanvas.renderMode = RenderMode.WorldSpace;
        //adjust it's fucking coords because who the fuck cares what I entered in the inspector, toss it out at the first opportunity
        //and make me do it again. Keep the original Z value though - it's magic and I don't want to poke it lest I invite the wrath of god
        myCanvas.transform.localPosition = new Vector3(infoCanvasOffset.x, infoCanvasOffset.y, myCanvas.transform.localPosition.z);
        myBuffCanvas.transform.localPosition = new Vector3(infoCanvasOffset.x, infoCanvasOffset.y, myBuffCanvas.transform.localPosition.z);
        //we also gotta adjust it's scale - the one it chooses by itself is a bit too large
        myCanvas.transform.localScale = new Vector3(infoCanvasScale, infoCanvasScale, 0.0f);
        myBuffCanvas.transform.localScale = new Vector3(infoCanvasScale, infoCanvasScale, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        //handle drag-&-drop
        if (dragging && SoupManager.main.AllowInteraction)
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
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        //abort if player interaction is disabled
        if (!SoupManager.main.AllowInteraction)
        {
            return;
        }
        //another abort, this time to prevent non-removal interactions while only removal from soup is allowed (for the tutorial)
        if (!inSoup && SoupManager.main.AllowOnlyRemove)
        {
            return;
        }

    	//when clicked, start dragging the object
    	dragging = true;
    	//if we're in the soup, remove ourselves
    	if(inSoup)
    	{
    		pot.RemoveIngredient(this); //get out of the pot. Assume the pot will handle everything with the soup manager.
            buffUI.SetActive(false); //turn off buff UI
            hoverUI.SetActive(true); //turn on hover UI
    		inSoup = false;
    	}
        else
        {
            // play correct sfx based on which ingredience
            switch(ingredient.name)
            {
                case "Cabbage":
                    sfxCabbage.Play();
                    break;
                case "Carrot":
                    sfxCarrots.Play();
                    break;
                case "Chicken":
                    sfxChicken.Play();
                    break;
                case "Mushroom":
                    sfxMushroom.Play();
                    break;
                case "Noodles":
                    sfxNoodles.Play();
                    break;
                case "Onions":
                    sfxOnions.Play();
                    break;
                case "Potato":
                    sfxPotatoes.Play();
                    break;
                case "Tomato":
                    sfxTomatoes.Play();
                    break;
                case "Walnuts":
                    sfxWalnuts.Play();
                    break;
            }
        }
    }

    //if unclicked, stop dragging
    private void OnMouseUp()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        //abort if player interaction is disabled
        if (!SoupManager.main.AllowInteraction)
        {
            return;
        }
        //another abort, this time to prevent non-removal interactions while only removal from soup is allowed (for the tutorial)
        if (!inSoup && !dragging && SoupManager.main.AllowOnlyRemove)
        {
            return;
        }

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
                buffUI.SetActive(true); //turn on the buff UI
                hoverUI.SetActive(false); //turn off the hover UI
    		}
    		else
    		{
    			//no more room
    			Debug.Log("Oh no! Pot is full");
    			ResetPosition();//transform.position = startPos; //go back home and cry because the popular kids rejected us yet again
    		}
    	}
    	else
    	{
    		//we were not dropped in the soup
    		ResetPosition();//transform.position = startPos; //go back home and cry because you missed the pot
            SoupEvents.main.threeIngredients._event.Invoke(); //attempt to trigger tutorial progression
    	}
    }

    //detect mouse over & display the target/effect data
    private void OnMouseEnter()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        //abort if player interaction is disabled
        if (!SoupManager.main.AllowInteraction)
        {
            return;
        }

    	//hoverUI.SetActive(true); //turn on UI
        sfxHighlight.Play();
        if (!inSoup && !SoupManager.main.AllowOnlyRemove)
        {
            hoverUI.SetActive(true);
        }
    }

    //detect mouse no longer over & hide the target/effect data
    private void OnMouseExit()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        //abort if player interaction is disabled
        if (!SoupManager.main.AllowInteraction)
        {
            return;
        }

    	if(!inSoup)
    	{
    		hoverUI.SetActive(false); //turn off UI
    	}
    }

    /// <summary>
    /// Updates images & text in the event that the ingredient changes.
    /// </summary>
    public void UpdateData()
    {
    	Debug.Log(ingredient.name + ", " + myCanvas);
        //set effect text based on ingredient
        Text effectTxt = myCanvas.transform.Find("EffectText").GetComponent<Text>();
        effectTxt.text = ingredient.GetEffectText();
        //set ingredient icon based on ingredient
        SpriteRenderer ingIcon = GetComponent<SpriteRenderer>();
        ingIcon.sprite = ingredient.ingredientIcon;
    	//set character icon based on ingredient
        // Image charIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        Image charIcon = myCanvas.transform.Find("CharacterIcon").GetComponent<Image>();
        charIcon.sprite = ingredient.characterIcon;
        Image buffIcon = myBuffCanvas.transform.Find("CharacterIcon").GetComponent<Image>();
        buffIcon.sprite = ingredient.characterIcon;
    }

    /// <summary>
    /// Resets the ingredient to its initial position (stored in startPos)
    /// </summary>
    public void ResetPosition()
    {
    	transform.position = startPos;
    	inSoup = false; //it's safe to assume that if we're resetting our position, we're not in the soup
    	hoverUI.SetActive(false); //turn off the hover UI too
        sfxCancel.Play();
    }

    /// <summary>
    /// Updates the buff UI with the base stats and active buffs.
    /// Args: baseHP/baseFP are the current base stats of the target (retrieved from the prefab),
    ///       doHPBuff/doFPBuff represent whether to apply the HP/FP buff effect and show the "buff active" arrow 
    /// </summary>
    public void UpdateBuffUI(int baseHP, int baseFP, bool doHPBuff, bool doFPBuff)
    {
        //get references to the HP/FP text objects
        Text hpText = myBuffCanvas.transform.Find("HPText").GetComponent<Text>();
        Text fpText = myBuffCanvas.transform.Find("FPText").GetComponent<Text>();
        //get references to the HP/FP arrows and disable them (they'll be activated again if their buff is actually active)
        GameObject hpArrow = myBuffCanvas.transform.Find("HPArrow").gameObject;
        hpArrow.SetActive(false);
        GameObject fpArrow = myBuffCanvas.transform.Find("FPArrow").gameObject;
        fpArrow.SetActive(false);

        //apply the active buffs
        if (doHPBuff)
        {
            baseHP += 2;
            hpArrow.SetActive(true);
        }
        if (doFPBuff)
        {
            baseFP += 1;
            fpArrow.SetActive(true);
        }

        //display the final HP/FP values
        hpText.text = "" + baseHP;
        fpText.text = "" + baseFP;
    }

    public void HideBuffUI()
    {
        buffUI.SetActive(false);
    }
}
