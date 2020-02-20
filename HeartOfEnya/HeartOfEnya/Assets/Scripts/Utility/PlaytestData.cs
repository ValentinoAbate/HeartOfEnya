using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that defines the data that will be written to the playtest log .csv file
/// </summary>
public class PlaytestData : MonoBehaviour
{
    private string name;
    private uint amountOfLoveForBapy;

    /// <summary>
    /// Updates the internal playtest data with new values
    /// </summary>
    public void NewDataLog(string _name, uint _love)
    {
        name = _name;
        amountOfLoveForBapy = _love;
    }

    public string FieldNames()
    {
        return "Name,Amount Of Love For Bapy";
    }

    public override string ToString()
    {
        string output = name + "," + amountOfLoveForBapy;
        return output;
    }
}
