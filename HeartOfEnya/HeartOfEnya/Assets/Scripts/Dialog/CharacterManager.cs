using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //Globally-accessible instance used to access the data.
	public static CharacterManager main;

    //PhaseData
    public PhaseData phaseDataA;
    public PhaseData phaseDataB;
    public PhaseData phaseDataC;
    public PhaseData phaseDataD;
    public PhaseData phaseDataE;

    //list of days the party levels up (in the format of phase letter + day number, e.g. A0)
    public List<string> levelUpDates;

    private Dictionary<string, PhaseData> phaseDict;

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
            phaseDict = new Dictionary<string, PhaseData>()
            {
                {"A", phaseDataA },
                {"B", phaseDataB },
                {"C", phaseDataC },
                {"D", phaseDataD },
                {"E", phaseDataE },
            };
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

    public PhaseData GetPhaseData(string gamePhase) => phaseDict[gamePhase];

    [System.Serializable]
    public class PhaseData
    {
        public string monologCharacter;
        public string monologNode;
        public string monologMusic;
        public string campfireNode;
        public string campfireMusic;
        public SceneDict extraScenes;       
    }
    [System.Serializable]
    public class SceneDict : SerializableCollections.SDictionary<string, string> { }
}
