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
    public string VoiceEvent { get => data.voiceEvent; }
    
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
            var phaseData = CharacterManager.main.GetPhaseData(phase);

            if(phaseData.monologCharacter.ToLower() == Name.ToLower())
            {
                dialogManager.StartCampMonolog(phaseData);
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

    
