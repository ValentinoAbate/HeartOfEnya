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
    private int wave;       // which wave is it
    private int day;        // which day is it
    private int numEnemies; // number of enemies spawned in the wave
    private string victory; // whether player won the wave or retreated
    private int turns;      // how many turns it took to finish wave

    public Dictionary<string, int> hp = new Dictionary<string, int>();      // how much hp/fp each char lost
    public Dictionary<string, int> fp = new Dictionary<string, int>();
    public Dictionary<string, int> moves = new Dictionary<string, int>();   // how many times each move was used
    public Dictionary<string, int> moveDmg = new Dictionary<string, int>(); // how much damage each move did
    
    public int obstaclesDestroyed;                    // how many obstacles were destroyed
    public List<int> obstacleTurns = new List<int>(); // which turns obstacles were destroyed on
    
    public float avgEnemies;   // average number of enemies on screen
    public Dictionary<string, int> enemyDmg = new Dictionary<string, int>(); // how much damage each enemy time dealt
    public int stunnedEnemies; // how many times an enemy was stunned
    public float avgPos;       // average x position of all characters

    private void Awake()
    {
        hp["Bapy"] = 0;
        hp["Soleil"] = 0;
        hp["Raina"] = 0;
        hp["Lua"] = 0;

        fp["Bapy"] = 0;
        fp["Soleil"] = 0;
        fp["Raina"] = 0;
        fp["Lua"] = 0;
    }


    /// <summary>
    /// Updates the internal playtest data with new values
    /// </summary>
    public void NewDataLog(int _wave, int _day, int enemies, string _victory)
    {
        wave = _wave;
        day = _day;
        numEnemies = enemies;
        victory = _victory;
    }

    public void UpdateTurnCount(int turn)
    {
        turns = turn;
    }

    public string FieldNames()
    {
        string names = "Wave,Day,Number of Enemies,Victory,Turns,HP Lost,FP Used,Moves Used,Move Damage,Obstacles Destroyed" +
                       ",Obstacle Destruction Turns,Average Enemies,Enemy Damage,Stunned Enemies,Average Party Column";
        return names;
    }

    public override string ToString()
    {
        // string output = name + "," + amountOfLoveForBapy;
        string output = wave.ToString() + "," + day.ToString() + "," + numEnemies.ToString() + ",";
        output += victory + "," + turns.ToString() + ",";

        output += PrintDictionary(hp) + "," + PrintDictionary(fp) + ",";
        output += PrintDictionary(moves) + "," + PrintDictionary(moveDmg) + ",";
        output += obstaclesDestroyed.ToString() + "," + PrintList(obstacleTurns);

        output += avgEnemies.ToString() + "," + PrintDictionary(enemyDmg) + "," + stunnedEnemies.ToString(); 
        output += "," + avgPos.ToString();

        return output;
    }

    private string PrintDictionary(Dictionary<string, int> dictionary)
    {
        string output = "\""; // csv entries containing commas have to be in quotes

        // iterate through dictionary entries
        foreach(KeyValuePair<string, int> entry in dictionary)
        {
            // add entry to output string
            output += entry.Key + ": " + entry.Value + ", ";
        }

        output += "\"";
        return output;
    }

    private string PrintList(List<int> list)
    {
        string output = "\"";

        // iterate through list items
        foreach(int i in list)
        {
            // add item to output string
            output += i.ToString() + ", ";
        }

        output += "\"";
        return output;
    }
}
