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

    private FMODUnity.StudioEventEmitter sfxSelect;

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
    public AudioClip TextBlip { get => data.textScrollSfx; }
    
    private void Awake()
    {
        Expression = defaultExpression;
        sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
    }

    //runs whenever the character gets clicked on
    void OnMouseDown()
    {
    	Debug.Log("You've clicked on: " + Name);
    	
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

    		//See if we have a script for today, and if so run it
    		string scriptName = CharacterManager.main.GetScript(Name, phase, day);
    		if (scriptName == null)
    		{
    			//If characters should do anything if they don't have a script, put it here
    			//(e.g. run a default script, play a "Willow noticed me" animation, etc.)
    			Debug.Log(Name + " has no script for phase " + phase + ", day " + day);
    		}
    		else
    		{
    			//character has an appropriate script - run it
    			Debug.Log(Name + " launches script " + scriptName);
    			//dialogManager.StartDialogue(scriptname); //disabled for now because we only have the one script
    		}

    		/***TEMP CODE***/
    		//launches the default script. SHOULD BE DELETED ONCE THE ABOVE CODE IS FINALIZED.
    		dialogManager.StartDialogue(); //only exists to preserve gameplay progression
    	}
    }
}

    
