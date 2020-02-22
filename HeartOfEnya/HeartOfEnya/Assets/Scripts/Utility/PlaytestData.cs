using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that defines the data that will be written to the playtest log .csv file
/// </summary>
public class PlaytestData : MonoBehaviour
{
    // private string name;
    // private uint amountOfLoveForBapy;
    public int wave;       // which wave is it
    public int day;        // which day is it
    public int numEnemies; // number of enemies spawned in the wave
    public string victory; // whether player won the wave or retreated
    public int turns;      // how many turns it took to finish wave

    public Dictionary<string, int> hp = new Dictionary<string, int>();    // how much hp/fp each char lost
    public Dictionary<string, int> fp = new Dictionary<string, int>();
    public Dictionary<string, int> moves = new Dictionary<string, int>(); // how many times each move was used
    
    public int obstaclesDestroyed;                    // how many obstacles were destroyed
    public List<int> obstacleTurns = new List<int>(); // which turns obstacles were destroyed on
    
    public float avgEnemies;   // average number of enemies on screen
    public Dictionary<string, int> enemyDmg = new Dictionary<string, int>(); // how much damage each enemy time dealt
    public int stunnedEnemies; // how many times an enemy was stunned
    public float avgPos;       // average x position of all characters


    /// <summary>
    /// Updates the internal playtest data with new values
    /// </summary>
    // public void NewDataLog(string _name, uint _love)
    // {
    //     // name = _name;
    //     // amountOfLoveForBapy = _love;
    // }

    public string FieldNames()
    {
        return "Name,Amount Of Love For Bapy";
    }

    public override string ToString()
    {
        // string output = name + "," + amountOfLoveForBapy;
        return "";
    }
}
