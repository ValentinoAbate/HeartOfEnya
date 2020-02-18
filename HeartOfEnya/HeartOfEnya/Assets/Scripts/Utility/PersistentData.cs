using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains all the data that has to be saved between states
/// GameState Legend:
/// "A"
/// </summary>
public class PersistentData : MonoBehaviour
{
    public const string gamePhaseLuaBattle = "B";
    public const string gamePhaseAbsoluteZeroBattle = "E";

    [Header("Battle")]
    public int partyLevel;
    public Dictionary<string, int> wounds;  // number of times character was hit at 0 hp,
                                            // keys are character names e.g. wounds["Bapy"]
    public bool luaBossDefeated;
    public bool absoluteZeroDefeated;
    public int numEnemiesLeft;
    public List<Enemy> listEnemiesLeft;     // data of remaining enemies
    // public List<buffs?> buffStructures;  // buffs characters are taking into battle

    [Header("Dialog")]
    public string gamePhase = "A"; // GamePhase: What “day” it is.
    public int dayNum = 0;       // DayNum: The number of days spent on the current Phase
    
    [Header("Soup")]
    public List<string> gatheredIngredients;  // List of gathered ingredients
    // public List<Soup> soupPool;            // List of soups in inventory

    private void Awake()
    {
        // initialize variables
        wounds = new Dictionary<string, int>();
        listEnemiesLeft = new List<Enemy>();
        // buffStructures = new List<
        gatheredIngredients = new List<string>();
        // soupPool = new List<Soup
    }
}
