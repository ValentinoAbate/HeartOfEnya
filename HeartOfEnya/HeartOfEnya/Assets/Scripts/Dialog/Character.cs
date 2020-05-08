using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Character : MonoBehaviour
{
    public const string defaultExpression = "normal";
    public static readonly Color dimColor = Color.gray;
    public string Name { get => characterName; }
    [SerializeField] private string characterName;
    public Vector2 DialogSpawnPoint => dialogSpawnPoint.position;
    [SerializeField] private Transform dialogSpawnPoint;
    [SerializeField] private CharacterData data;
    [SerializeField] private DialogueRunner dialogManager;
    [SerializeField] private Vector3 doorPosition;	//where to put the sprite during our monologue night
    [SerializeField] private Sprite doorSprite;
    [SerializeField] private Vector3 doorButtonPos; //where to put the button during out monologue night
    [SerializeField] private Image fadeImage;   //used in the fade-to-black transitions
    [SerializeField] private float fadeSpeed = 1.5f; //controls the speed of fade-to-black transitions

    private FMODUnity.StudioEventEmitter sfxSelect;
    private Animator anim;

    public string Expression
    {
        get => expression;
        set
        {
            if (data.portraits.ContainsKey(value))
            {
                expression = value;
                Portrait = data.portraits[value];
            }
            else
                Debug.LogError(value + " is not a valid expression for " + Name);
        }
    }
    private string expression;
    public Sprite Portrait { get; private set; }
    public string VoiceEvent { get => data.voiceEvent; }

    private void Awake()
    {
        Expression = defaultExpression;
        sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        anim = GetComponent<Animator>();

        //set up the fade-to-black image
        fadeImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeImage.enabled = false; //disable temporarily
    }

    private void Start()
    {
        if (CharacterManager.main == null)
            return;
        //retrieve date from persistent data
        string phase = DoNotDestroyOnLoad.Instance.persistentData.gamePhase;
        var phaseData = CharacterManager.main.GetPhaseData(phase);
        //move to the door position if it's our monologue time
        if (phaseData != null && phaseData.monologCharacter.ToLower() == Name.ToLower())
        {
            //on the monologue day, move to the door
            if (DoNotDestroyOnLoad.Instance.persistentData.dayNum == 0)
            {
                //swap the sprite to the door sprite
                SpriteRenderer icon = transform.Find("Sprite").GetComponent<SpriteRenderer>();
                icon.sprite = doorSprite;
                //move the sprite to the door
                transform.position = doorPosition;
                //move the button to the door
                var button = transform.Find("Canvas").transform.Find("Button"); //Button is child of Canvas, which is child of the CampCharacter
                button.transform.localPosition = doorButtonPos;
            }

            if(anim != null)
                    anim.SetBool("Highlight", true);
        }
    }

    //runs whenever the character gets clicked on
    public void RunDialogue()
    {
    	Debug.Log("You've clicked on: " + Name + " on day " + DoNotDestroyOnLoad.Instance.persistentData.dayNum);

    	//if the dialog is already running, don't do anything
    	if (dialogManager.isDialogueRunning)
    	{
    		Debug.Log(Name + " won't interrupt the current conversation");
    		return;
    	}
    	else
    	{
    		sfxSelect.Play();

            //retrieve date from persistent data
    		string phase = DoNotDestroyOnLoad.Instance.persistentData.gamePhase;
    		int day = DoNotDestroyOnLoad.Instance.persistentData.dayNum;
            var phaseData = CharacterManager.main.GetPhaseData(phase);

            if(phaseData.monologCharacter.ToLower() == Name.ToLower()) //if the clicked character's monologue takes place in this scene
	        {
                if (anim != null)
                {
                    anim.SetBool("Highlight", false);
                }
                if (day == 0)
	        	{
	        		//If it's day 0 (i.e. 1st night in this phase), launch their monologue
	            	Debug.Log(Name + " launches their monologue");
                    
                    //transition to the solo monologue
                    StartCoroutine(DoMonologueTransition(phase));

                    //start the dialogue
	            	// dialogManager.StartCampMonolog(phaseData);
	           	}
	            else if (day == 1)
	            {
	            	//If it's day 1 (i.e. 2nd night in this phase), launch their party scene
	            	Debug.Log(Name + " launches their party scene");
	        		dialogManager.StartCampPartyScene(phaseData);
	            }
	            else
	            {
	            	//If it's day 2 or higher, look for "filler" dialogue
	            	if(phaseData.extraScenes.ContainsKey(Name))
	            	{
	            		string scriptName = phaseData.extraScenes[Name];
	            		//character has an appropriate script - run it
	            		Debug.Log(Name + " launches script " + scriptName);
	            		dialogManager.StartDialogue(scriptName);
	            	}
	            }
            }
            else if(phaseData.extraScenes.ContainsKey(Name))
            {
                string scriptName = phaseData.extraScenes[Name];
                //character has an appropriate script - run it
                Debug.Log(Name + " launches script " + scriptName);
                dialogManager.StartDialogue(scriptName);
            }
            else if(phaseData.extraScenes.ContainsKey(Name + day))
            {
                string scriptName = phaseData.extraScenes[Name + day];
                //character has an appropriate script - run it
                Debug.Log(Name + " launches script " + scriptName);
                dialogManager.StartDialogue(scriptName);
            }
    	}
    }

    public IEnumerator DoMonologueTransition(string phase)
    {
        //fade to black
        yield return StartCoroutine("FadeToBlack");
        //replace background prefab with monologue prefab
        Debug.Log("SWAP BACKGROUND");
        //remove other characters
        Debug.Log("YEET OTHER CHARACTERS");
        //fade in
        yield return StartCoroutine("FadeToClear");
        //run the dialogue
        var phaseData = CharacterManager.main.GetPhaseData(phase); //can't pass phaseData to the coroutine so let's just get it again
        dialogManager.StartCampMonolog(phaseData);
    }

    public IEnumerator FadeToBlack()
    {
        fadeImage.enabled = true; //re-enable for the fade-out
        float elapsedTime = 0; //keep track of how long we've been here
        do
        {
            fadeImage.color = Color.Lerp(Color.clear, Color.black, fadeSpeed * elapsedTime);
            //Debug.Log("ALPHA: " + fadeImage.color.a);
            elapsedTime += Time.deltaTime;
            if (fadeImage.color.a >= 0.95f) //if we're nearly opaque, go fully opaque and stop
            {
                fadeImage.color = Color.black;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        while (true);
    }

    public IEnumerator FadeToClear()
    {
        fadeImage.enabled = true; //re-enable for the fade-in
        float elapsedTime = 0; //keep track of how long we've been here
        do
        {
            fadeImage.color = Color.Lerp(Color.black, Color.clear, fadeSpeed * elapsedTime);
            //Debug.Log("ALPHA: " + fadeImage.color.a);
            elapsedTime += Time.deltaTime;
            if (fadeImage.color.a <= 0.05f) //if we're nearly transparent, go fully transparent, disable the image, and stop
            {
                fadeImage.color = Color.clear;
                fadeImage.enabled = false;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        while (true);
    }
}
