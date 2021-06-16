using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public class PersistentSaveData : MonoBehaviour
{
    private const string fileName = "PersistentSaveData.txt";
    public string LatestGamePhase => latestGamePhase;
    [SerializeField] private string latestGamePhase = PersistentData.gamePhaseIntro;
    public bool OnBattle => onBattle;
    [SerializeField] private bool onBattle = true;
    public int LatestDay => latestDay;
    [SerializeField] private int latestDay = PersistentData.dayNumStart;
    private bool loaded = false;
    public void UpdatePersistantSaveData(bool camp)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if(latestGamePhase == pData.gamePhase)
        {
            if (latestDay < pData.dayNum)
                latestDay = pData.dayNum;
            onBattle = !camp || (pData.InLuaBattle && !pData.luaBossPhase2Defeated) || (pData.InAbs0Battle && !pData.absoluteZeroPhase1Defeated);
        }
        else if(PersistentData.GamePhaseToInt(latestGamePhase) < PersistentData.GamePhaseToInt(pData.gamePhase))
        {
            latestGamePhase = pData.gamePhase;
            latestDay = pData.dayNum;
            onBattle = true;
        }
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, PersistentData.saveDataPath);
        savePath = Path.Combine(savePath, fileName);

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
            Debug.Log("Saved data to: " + savePath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to save data to " + savePath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    //Loads pdata from file. Used in game saving.
    //Based on https://stackoverflow.com/questions/40965645/what-is-the-best-way-to-save-game-state/40966346#40966346
    private void LoadFromFile()
    {
        if (loaded)
            return;
        loaded = true;
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, PersistentData.saveDataPath);
        savePath = Path.Combine(savePath, fileName); //naughty ben, hardcoding filenames! Should probably fix later

        //abort if the save file and/or directory doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Debug.LogWarning("Cannot load persistent save data - save directory doesn't exist!");
            return;
        }
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Cannot load persistent save data - save file doesn't exist!");
            return;
        }

        //load the saved JSON
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(savePath);
            Debug.Log("Loaded data from " + savePath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load data from " + savePath.Replace("/", "\\"));
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
    }

    private void Start()
    {
        LoadFromFile();
    }
}
