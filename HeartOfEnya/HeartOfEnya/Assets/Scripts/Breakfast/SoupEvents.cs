using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yarn.Unity;

public class SoupEvents : MonoBehaviour
{
    // struct wrapper for unity event containing flag
    [System.Serializable]
    public struct SoupEvent
    {
        public UnityEvent _event;
        public bool flag;
    }

    //enum for tutorial state tracking
    private enum TutorialState
    {
        grabIngredient,
        buffExplain,
        removeIngredient,
        threeIngredients,
        experiment,
        makeSoup
    }

    public static SoupEvents main;
    [HideInInspector] public bool tutorialDay2;

    [Header("Tutorial Text")]
    public string grabIngredientText; //prompt the user to drag & drop ingredients into the pot
    public string buffExplainText; //explain the buff effects
    public string removeIngredientText; //prompt the user to remove ingredients
    public string threeIngredientText; //tell user about the three-ingredient limit
    public string experimentText; //tell the user to experiment with different combos
    public string makeSoupText; //prompt user to press the "make soup" button to confirm recipe

    [Header("Tutorial Events")]
    public SoupEvent tutorialIntro;
    public SoupEvent buffExplanation;
    public SoupEvent removeIngredient;
    public SoupEvent threeIngredients;
    public SoupEvent experiment;
    public SoupEvent makeSoup;

    //references to UI elements we'll have to interface with
    private GameObject tutorialTextBox; //the textbox where written instructions are displayed, including both the text and background images
    private Text tutorialText; //the actual text object where the instructions are written. Gets its own reference because it gets accessed a lot.
    private Image ingListArrow; //arrow pointing to the ingredients, used in step 1
    private Image buffUIArrow; //arrow pointing to the active buff UI, used in step 2
    private Image potArrow; //arrow pointing to the active ingredient, used in step 3
    private Image makeSoupArrow; //arrow pointing to the "make soup" button, used in step 6

    //tutorial state tracking
    private TutorialState tutorialState;

    private void Awake()
    {
        Debug.Log("AWAKE");
        if (main == null)
        {
            main = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        //set up the UI element references
        var tutorialCanvas = transform.Find("TutorialCanvas"); //get a reference to their parent canvas to reduce the # of transform.find() calls
        tutorialTextBox = tutorialCanvas.transform.Find("TextBox").gameObject;
        tutorialText = tutorialTextBox.transform.Find("TutText").GetComponent<Text>();
        ingListArrow = tutorialCanvas.transform.Find("IngListArrow").GetComponent<Image>();
        buffUIArrow = tutorialCanvas.transform.Find("BuffUIArrow").GetComponent<Image>();
        potArrow = tutorialCanvas.transform.Find("PotArrow").GetComponent<Image>();
        makeSoupArrow = tutorialCanvas.transform.Find("MakeSoupArrow").GetComponent<Image>();
        //disable all UI elements - assume they're not needed until proven otherwise
        ClearUI();
        //other setup
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        tutorialDay2 = pData.InTutorialSecondDay;
        tutorialState = TutorialState.grabIngredient;
        Debug.Log("EVENTS LOADING " + tutorialDay2);
    }

    // Update is called once per frame
    void Update()
    {
        if (SoupManager.main.PauseHandle.Paused)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            AdvanceTutorial();
        }
    }

    //Disables all UI elements. Intended for use in switching between tutorial states.
    private void ClearUI()
    {
        tutorialTextBox.SetActive(false);
        ingListArrow.enabled = false;
        buffUIArrow.enabled = false;
        potArrow.enabled = false;
        makeSoupArrow.enabled = false;
    }

    //used in handling click-to-advance transitions
    private void AdvanceTutorial()
    {
        //figure out which transition to do
        switch (tutorialState)
        {
            case TutorialState.removeIngredient: //transition to the removeIngredient state
                main.removeIngredient._event.Invoke();
                break;
            case TutorialState.experiment: //transition to the experiment state
                main.experiment._event.Invoke();
                break;
            case TutorialState.makeSoup: //transition to the makeSoup state
                main.makeSoup._event.Invoke();
                break;
            default:
                //Debug.Log("User click registered, but there's no transition for this state");
                break;
        }
    }

    public void IntroTrigger()
    {
        if(tutorialDay2 && !tutorialIntro.flag)
        {
            Debug.Log("Soup triggers event: start of tutorial");
            tutorialIntro.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = grabIngredientText;
            ingListArrow.enabled = true;
            //set up for the next step
            tutorialState = TutorialState.buffExplain;
        }
    }

    public void BuffExplainTrigger()
    {
        if(tutorialDay2 && !buffExplanation.flag)
        {
            Debug.Log("Soup triggers event: explanation of buffs");
            buffExplanation.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = buffExplainText;
            buffUIArrow.enabled = true;
            //set up for the next step
            tutorialState = TutorialState.removeIngredient;
            SoupManager.main.AllowInteraction = false; //disable player interaction until they complete the next step
        }
    }

    public void RemoveIngredientTrigger()
    {
        if(tutorialDay2 && !removeIngredient.flag)
        {
            Debug.Log("Soup triggers event: removing ingredients");
            removeIngredient.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = removeIngredientText;
            potArrow.enabled = true;
            //set up for the next step
            tutorialState = TutorialState.threeIngredients;
            SoupManager.main.AllowInteraction = true; //re-enable player interaction...
            SoupManager.main.AllowOnlyRemove = true; //...but then blacklist every action except removing ingredients
        }
    }

    public void ThreeIngredientsTrigger()
    {
        if(tutorialDay2 && !threeIngredients.flag && removeIngredient.flag)
        {
            Debug.Log("Soup triggers event: explain 3 ingredients");
            threeIngredients.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = threeIngredientText;
            //set up for the next step
            tutorialState = TutorialState.experiment;
            SoupManager.main.AllowOnlyRemove = false; //disable the remove-only blacklist...
            SoupManager.main.AllowInteraction = false; //...but then disable ALL interaction
        }
    }

    public void ExperimentTrigger()
    {
        if(tutorialDay2 && !experiment.flag)
        {
            Debug.Log("Soup triggers event: encourage experimentation");
            experiment.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = experimentText;
            //set up for the next step
            tutorialState = TutorialState.makeSoup;
        }
    }

    public void MakeSoupTrigger()
    {
        if(tutorialDay2 && !makeSoup.flag)
        {
            Debug.Log("Soup triggers event: make soup");
            makeSoup.flag = true; //disable this event once it triggers
            //display the UI elements
            ClearUI(); //wipe the UI clean so we don't have to worry about other stages' UI
            tutorialTextBox.SetActive(true);
            tutorialText.text = makeSoupText;
            makeSoupArrow.enabled = true;
            //set up for the next step
            //tutorialState = TutorialState.makeSoup;
            SoupManager.main.AllowInteraction = true;
        }
    }
}
