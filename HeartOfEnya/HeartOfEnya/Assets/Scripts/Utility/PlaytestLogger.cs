using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Writes playtest data into a .csv spreadsheet file
/// </summary>
public class PlaytestLogger : MonoBehaviour
{
    public string FileName
    {
        get
        {
            if (Application.isEditor)
                return fileNameEditor;
            else
                return Application.persistentDataPath + fileNamePlayer;
        }
    }
    [SerializeField]
    private string fileNameEditor = "Assets/PlaytestLog.csv";
    [SerializeField]
    private string fileNamePlayer = "PlaytestLog.csv";
    public PlaytestData testData;

    /// <summary>
    /// Initializes the top row of the .csv file with the field names
    /// NOTE: should only be called once to create the file, calling it again will
    ///       delete all previous contents from the file
    /// </summary>
    public void InitializeLog(PlaytestData entry)
    {        
        using(StreamWriter writer = File.CreateText(FileName))
        {
            writer.WriteLine(testData.FieldNames());
            Debug.Log("Playtest log file initialized");
        }  
    }

    /// <summary>
    /// Takes a new PlaytestData entry as input and writes it to the .csv file
    /// </summary>
    public void LogData(PlaytestData entry)
    {
        if (!File.Exists(FileName))
            InitializeLog(entry);
        using(StreamWriter writer = File.AppendText(FileName))
        {
            writer.WriteLine(testData.ToString());
            Debug.Log("New playtest data entry logged: " + testData.ToString());
            testData.ResetData();
        }    
    }
}
