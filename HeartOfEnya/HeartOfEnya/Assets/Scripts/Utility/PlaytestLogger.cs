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
    public string fileName;
    public PlaytestData testData;

    /// <summary>
    /// Initializes the top row of the .csv file with the field names
    /// NOTE: should only be called once to create the file, calling it again will
    ///       delete all previous contents from the file
    /// </summary>
    public void InitializeLog(PlaytestData entry)
    {
        using(StreamWriter writer = File.CreateText(fileName))
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
        using(StreamWriter writer = File.AppendText(fileName))
        {
            writer.WriteLine(testData.ToString());
            Debug.Log("New playtest data entry logged: " + testData.ToString());
        }    
    }

    // Start function for debugging only, shouldn't actually do anything
    public void Start()
    {
        InitializeLog(testData);

        // testData.NewDataLog("hana", 4294967295);
        // LogData(testData);
        // testData.NewDataLog("everyone else", 100000000);
        // LogData(testData);
    }
}
