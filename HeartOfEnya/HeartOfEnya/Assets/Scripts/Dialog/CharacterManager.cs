using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //Globally-accessible instance used to access the data.
	public static CharacterManager main;
    //list of days the party levels up (in the format of phase letter + day number, e.g. A0)
    public List<string> levelUpDates;
	//mappings of day to relevant script (editor-facing)
	public DayScriptPair[] bapyScripts;
	public DayScriptPair[] luaScripts;
	public DayScriptPair[] rainaScripts;
	public DayScriptPair[] soleilScripts;

	//mappings of day to relevant script (code-facing)
	private Dictionary<string, string> bapyDict = new Dictionary<string, string>();
	private Dictionary<string, string> luaDict = new Dictionary<string, string>();
	private Dictionary<string, string> rainaDict = new Dictionary<string, string>();
	private Dictionary<string, string> soleilDict = new Dictionary<string, string>();

	///A data structure used to match day codes to script names.
	///Only used to provide an editor-visible interface to the system.
	///Based on https://answers.unity.com/questions/497094/from-two-lists-into-a-dictionary-doesnt-serialize.html
	[System.Serializable]
	public class DayScriptPair
	{
		public string day;
		public string script;
	}

    private void Awake ()
    {
    	//implement the singleton pattern
    	if (main != null)
    	{
    		//if another singleton already exists, destroy the new one
    		Destroy(this.gameObject);
    	}
    	else
    	{
    		//if there's not already another singleton, we become the singleton
    		main = this;
    		//transform the editor-facing DayScriptPair arrays into more code-useful dictionaries
    		ConvertToDicts();
            //check if the party levels up today, and if so level them up
            var dateCode = DoNotDestroyOnLoad.Instance.persistentData.gamePhase + DoNotDestroyOnLoad.Instance.persistentData.dayNum;
            Debug.Log("WELCOME TO DAY " + dateCode);
            if (levelUpDates.Contains(dateCode))
            {
                Debug.Log("Leveling up characters...");
                DoNotDestroyOnLoad.Instance.persistentData.partyLevel += 1;
                Debug.Log("Characters are now level " + DoNotDestroyOnLoad.Instance.persistentData.partyLevel);
            }
    	}
    }

    ///Since Unity cannot serialize dictionaries, I've had to use the DayScriptPair arrays to give something that's
    ///visible in the editor. Since those are unwieldy in the code, we'll have to convert them into dictionaries on
    ///startup. Runs in O(n) time because you have to do each entry one-by-one.
    private void ConvertToDicts()
    {
    	for (int i = 0; i < bapyScripts.Length; i++)
    	{
    		bapyDict[bapyScripts[i].day] = bapyScripts[i].script;
    	}
    	for (int i = 0; i < luaScripts.Length; i++)
    	{
    		luaDict[luaScripts[i].day] = luaScripts[i].script;
    	}
    	for (int i = 0; i < rainaScripts.Length; i++)
    	{
    		rainaDict[rainaScripts[i].day] = rainaScripts[i].script;
    	}
    	for (int i = 0; i < soleilScripts.Length; i++)
    	{
    		soleilDict[soleilScripts[i].day] = soleilScripts[i].script;
    	}
    }

    ///GetScript will look up the character and "date" and convert that to the script to run.
    ///Returns null If no script is found.
    public string GetScript(string name, string gamePhase, int dayNum)
    {
    	//find the speaking character, then check if they have an entry for the current date 
    	string result = null; //stores the found script
    	switch(name)
    	{
    		case "Bapy":
    			result = SearchDict(bapyDict, gamePhase, dayNum);
    			break;
    		case "Lua":
    			result = SearchDict(luaDict, gamePhase, dayNum);
    			break;
    		case "Raina":
    			result = SearchDict(rainaDict, gamePhase, dayNum);
    			break;
    		case "Soleil":
    			result = SearchDict(soleilDict, gamePhase, dayNum);
    			break;
    		default:
    			Debug.LogError("CharacterManager: Unknown Character: " + name);
    			break;
    	}
    	return result;
    }

    ///SearchDict will search through a given character's date-script dictionary and find
    ///whichever script best fits the given date. Returns null if no appropriate script is found.
    private string SearchDict(Dictionary<string, string> charDict, string gamePhase, int dayNum)
    {
    	string date = gamePhase + dayNum; //assume all date-specific entries will be in this format

    	if (charDict.ContainsKey(date)) //1st, check if we have a script for this specific day
    	{
    		return charDict[date];
    	}
    	else if (charDict.ContainsKey(gamePhase + "General")) //next, check if we have a general script for this phase
    	{
   			return charDict[gamePhase + "General"];
    	}
   		else if (charDict.ContainsKey("Default")) //last, see if we have a basic script
    	{
    		return charDict["Default"];
    	}
    	else //no scripts found
    	{
    		return null;
    	}
    }
}
