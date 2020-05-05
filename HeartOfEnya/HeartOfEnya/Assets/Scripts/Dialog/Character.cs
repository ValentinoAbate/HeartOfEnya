using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private Vector3 doorPosition;	//where to go during our monologue night
    [SerializeField] private bool doorOverride;		//temporary control for going to the door - will be removed once I hook the system up to game time

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
    }

    private void Start()
    {
        if (CharacterManager.main == null)
            return;
        //retrieve date from persistent data
        string phase = DoNotDestroyOnLoad.Instance.persistentData.gamePhase;
        var phaseData = CharacterManager.main.GetPhaseData(phase);
        //move to the door position if it's our monologue time
        if (phaseData != null && phaseData.monologCharacter.ToLower() == Name.ToLower()) //second parameter makes sure we only trigger in the VN scene
        {
        	transform.position = doorPosition;
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
	            	dialogManager.StartCampMonolog(phaseData);
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
}
