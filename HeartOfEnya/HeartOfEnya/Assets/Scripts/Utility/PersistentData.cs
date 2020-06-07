using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains all the data that has to be saved between states
/// GameState Legend:
/// "A"
/// </summary>
public class PersistentData : MonoBehaviour
{
    public const int dayNumStart = 0;
    public const string gamePhaseIntro = "INTRO";
    public const string gamePhaseTut1And2 = "A";
    public const string gamePhaseTut3AndLuaBattle = "B";
    public const string gamePhaseBeginMain = "C";
    public const string gamePhaseLuaUnfrozen = "D";
    public const string gamePhaseAbsoluteZeroBattle = "E";
    public const string gamePhaseLevelEditor = "LE";

    public bool InMainPhase => gamePhase == gamePhaseBeginMain || gamePhase == gamePhaseLuaUnfrozen;
    public bool LuaUnfrozen => gamePhase == gamePhaseLuaUnfrozen || gamePhase ==  gamePhaseAbsoluteZeroBattle;
    public bool InTutorialFirstDay => gamePhase == gamePhaseTut1And2 && dayNum == dayNumStart;
    public bool InTutorialSecondDay => gamePhase == gamePhaseTut1And2 && dayNum == dayNumStart + 1;
    public bool InTutorialThirdDay => gamePhase == gamePhaseTut3AndLuaBattle && dayNum == dayNumStart;
    public bool InTutorialMainPhase => gamePhase == gamePhaseBeginMain && dayNum == dayNumStart;
    public bool InTutorialLuaJoin => gamePhase ==gamePhaseLuaUnfrozen && dayNum == dayNumStart;
    public bool InLuaBattle => gamePhase == gamePhaseTut3AndLuaBattle && dayNum >= dayNumStart + 1;
    public bool InAbs0Battle => gamePhase == gamePhaseAbsoluteZeroBattle && !absoluteZeroPhase1Defeated;

    public int PartyLevel
    {
        get
        {
            if (gamePhase == gamePhaseLevelEditor)
                return levelEditorPartyLevel;
            if (LuaUnfrozen)
                return 4;
            if (gamePhase == gamePhaseBeginMain)
                return 3;
            if (gamePhase == gamePhaseTut3AndLuaBattle)
                return 2;
            return 1;
        }
    }

    [Header("Battle")]
    public bool luaBossPhase1Defeated;
    public bool luaBossPhase2Defeated;
    public bool absoluteZeroPhase1Defeated;
    public bool absoluteZeroPhase2Defeated;
    public int numEnemiesLeft; // Total in mainEncounter
    public int numEnemiesDefeatedThisEncounter = 0;
    public int waveNum;
    public Encounter lastEncounter; // Used to determing if we are on a new encounter or returning to an old one
    public List<SavedCombatant> listEnemiesLeft = new List<SavedCombatant>(); // data of remaining enemies
    public List<WaveData.SpawnData> listActiveSpawners = new List<WaveData.SpawnData>();
    public List<BuffStruct> buffStructures;  // buffs characters are taking into battle

    [Header("Dialog")]
    public string gamePhase = gamePhaseTut1And2;           // GamePhase: What “day” it is.
    public int dayNum = dayNumStart;         // DayNum: The number of days spent on the current Phase

    [Header("Level Editor")]
    public int levelEditorPartyLevel = PartyMember.minLevel;

    public string IntToGamePhase(int phaseNum)
    {
        if (phaseNum == 0)
            return gamePhaseIntro;
        return ((char)('A' + phaseNum - 1)).ToString();
    }

    [System.Serializable]
    public class SavedCombatant
    {
        public GameObject prefabAsset;
        public int remainingHP;
        public Pos spawnPos;
    }
}
