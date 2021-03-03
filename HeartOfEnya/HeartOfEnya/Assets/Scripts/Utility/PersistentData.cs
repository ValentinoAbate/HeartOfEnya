using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

/// <summary>
/// Class that contains all the data that has to be saved between states
/// GameState Legend:
/// "A"
/// </summary>
[System.Serializable]
public class PersistentData : MonoBehaviour
{
    public const string saveDataPath = "data";
    public const int dayNumStart = 0;
    public const string gamePhaseIntro = "INTRO";
    public const string gamePhaseTut1And2 = "A";
    public const string gamePhaseTut3AndLuaBattle = "B";
    public const string gamePhaseBeginMain = "C";
    public const string gamePhaseLuaUnfrozen = "D";
    public const string gamePhaseAbsoluteZeroBattle = "E";
    public const string gamePhaseGameComplete = "F";
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

    [Header("Saving")]
    public string returnScene; //what scene to return to on game load

    public static string IntToGamePhase(int phaseNum)
    {
        if (phaseNum == 0)
            return gamePhaseIntro;
        return ((char)('A' + phaseNum - 1)).ToString();
    }

    public static int GamePhaseToInt(string gamePhase)
    {
        if (gamePhase.ToUpper() == gamePhaseIntro)
            return 0;
        return (gamePhase.ToUpper()[0] - 'A') + 1;
    }

    [System.Serializable]
    public class SavedCombatant
    {
        public GameObject prefabAsset;
        public int remainingHP;
        public Pos spawnPos;
    }

    //Saves pdata to file. Used in game saving.
    //Based on https://stackoverflow.com/questions/40965645/what-is-the-best-way-to-save-game-state/40966346#40966346
    public void SaveToFile()
    {
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, saveDataPath);
        savePath = Path.Combine(savePath, "SaveData.txt"); //naughty ben, hardcoding filenames! Should probably fix later

        //convert to JSON, then to bytes
        string jsonData = JsonUtility.ToJson(this, true);
        byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData); //do this because I think the file writer expects a byte string or something

        //create the save directory if it doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        }

        //do the saving
        try
        {
            File.WriteAllBytes(savePath, jsonByte);
            Debug.Log("Saved data to: " + savePath.Replace("/","\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to save data to " + savePath.Replace("/","\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    //Loads pdata from file. Used in game saving.
    //Based on https://stackoverflow.com/questions/40965645/what-is-the-best-way-to-save-game-state/40966346#40966346
    public void LoadFromFile()
    {
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, saveDataPath);
        savePath = Path.Combine(savePath, "SaveData.txt"); //naughty ben, hardcoding filenames! Should probably fix later

        //abort if the save file and/or directory doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Debug.LogWarning("Cannot load save data - save directory doesn't exist!");
            return;
        }
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Cannot load save data - save file doesn't exist!");
            return;
        }

        //load the saved JSON
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(savePath);
            Debug.Log("Loaded data from " + savePath.Replace("/","\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load data from " + savePath.Replace("/","\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        //convert raw ASCII to JSON string, then to JSON object
        try
        {
            string jsonData = Encoding.ASCII.GetString(jsonByte);
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing save data: " + e.Message);
            
            /***TODO: add some way of communicating the error to the player & prompting them to load a new game***/
        }

        //load the scene we saved on
        if (String.IsNullOrEmpty(returnScene))
        {
            //if no return scene was specified, abort
            Debug.LogWarning("No return scene specified! Cannot complete load!");
        }
        else
        {
            SceneTransitionManager.main.TransitionScenes(returnScene);
        }
    }
}
