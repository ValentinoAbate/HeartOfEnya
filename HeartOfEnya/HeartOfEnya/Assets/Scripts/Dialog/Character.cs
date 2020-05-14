using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Character : MonoBehaviour
{
    public const string defaultExpression = "neutral";
    public static readonly Color dimColor = Color.gray;
    public string Name { get => characterName; }
    [SerializeField] private string characterName;
    public Vector2 DialogSpawnPoint => dialogSpawnPoint.position;
    public bool ignoreFixedPosition = false;
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
    public GameObject DialogBoxPrefab => data.dialogBoxPrefab;

    private void Awake()
    {
        Expression = defaultExpression;
        sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        anim = GetComponent<Animator>();

        //only set up the fade-to-black monologue transitions if we're in the camp scene
        if (fadeImage != null) //assume fadeImage will only have been set in the camp scene
        {
            //set up the fade-to-black image
            fadeImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
            fadeImage.enabled = false; //disable temporarily
        }
    }

    private void Start()
    {
        if (CharacterManager.main == null)
            return;
        //retrieve date from persistent data
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        string phase = pData.gamePhase;
        var phaseData = CharacterManager.main.GetPhaseData(phase);
        //move to the door position if it's our monologue time
        if (phaseData != null && !pData.absoluteZeroDefeated && phaseData.monologCharacter.ToLower() == Name.ToLower())
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
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            //retrieve date from persistent data
    		string phase = pData.gamePhase;
    		int day = pData.dayNum;
            var phaseData = CharacterManager.main.GetPhaseData(phase);

            if(phaseData.monologCharacter.ToLower() == Name.ToLower()) //if the clicked character's monologue takes place in this scene
	        {
                if (anim != null)
                {
                    anim.SetBool("Highlight", false);
                }
                if (day == 0 && phase != PersistentData.gamePhaseAbsoluteZeroBattle)
	        	{
	        		//If it's day 0 (i.e. 1st night in this phase), launch their monologue
	            	Debug.Log(Name + " launches their monologue");
                    
                    //transition to the solo monologue
                    if(fadeImage != null) //make sure we only trigger the monologue transition if the fadeout image is set properly
                    {
                        StartCoroutine(DoMonologueTransition(phase));
                    }
                    else
                    {
                        //If we don't have fadeImage, try to bypass the transition by performing the old "straight-to-dialogue" approach.
                        //This is mostly here in case some of the battle stuff relies on the original behavior.
                        Debug.LogWarning("ALERT: unable to transition to " + Name + "'s monologue due to lack of fadeImage.\nIf you're not in the camp scene, you can disregard this message.");
                        dialogManager.StartCampMonolog(phaseData);
                    }
	           	}
	            else
	            {
	            	//If it's day 1 or more (i.e. 2nd night in this phase), launch their party scene (unless this is a boss phase)
	            	Debug.Log(Name + " launches their party scene");
                    if((pData.InLuaBattle && !pData.luaBossDefeated) || 
                        (phase == PersistentData.gamePhaseAbsoluteZeroBattle && !pData.absoluteZeroDefeated))
                    {
                        if((phase == PersistentData.gamePhaseAbsoluteZeroBattle && day < 1) || (pData.InLuaBattle && day == 1))
                            dialogManager.StartDialogue(phaseData.bossWaitNode);
                        else
                            dialogManager.StartDialogue("BossFallback");
                    }
                    else
                    {
                        dialogManager.StartCampPartyScene(phaseData);
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

    //handles switching from the "normal" camp scene to the monologue version
    public IEnumerator DoMonologueTransition(string phase)
    {
        //fade to black
        yield return StartCoroutine("FadeToBlack");
        //replace background prefab with monologue prefab
        Debug.Log("SWAP BACKGROUND");
        //remove other characters
        Debug.Log("YEET OTHER CHARACTERS");
        DisableOtherCharacters();
        //fade in
        yield return StartCoroutine("FadeToClear");
        //run the dialogue
        var phaseData = CharacterManager.main.GetPhaseData(phase); //can't pass phaseData to the coroutine so let's just get it again
        dialogManager.StartCampMonolog(phaseData);
    }

    //handles fading the screen to black.
    //This and FadeToClear were based on code from https://gist.github.com/NovaSurfer/5f14e9153e7a2a07d7c5
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

    //handles fading the screen back in after it's been faded to black.
    //This and FadeToBlack were based on code from https://gist.github.com/NovaSurfer/5f14e9153e7a2a07d7c5
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

    public void DisableOtherCharacters()
    {
        GameObject[] toDisable = GameObject.FindGameObjectsWithTag("CampCharacter");
        foreach (GameObject otherCharacter in toDisable)
        {
            var otherScript = otherCharacter.GetComponent<Character>();
            //Debug.Log(theirScript.Name);
            if (otherScript.Name != Name)
            {
                Debug.Log(Name + " disabled " + otherScript.Name);
                otherCharacter.SetActive(false);
            }
            else
            {
                Debug.Log(Name + " won't disable themself");
            }
        }
    }
}
