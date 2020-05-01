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
        }
    }

    public PhaseData GetPhaseData(string gamePhase) => phaseDict.ContainsKey(gamePhase) ? phaseDict[gamePhase] : null;

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
