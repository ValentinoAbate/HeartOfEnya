using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    [Header("Battle")]
    public int partyLevel;
    public Dictionary<string, int> wounds;  // number of times character was hit at 0 hp,
                                            // keys are character names e.g. wounds["Bapy"] 
    public int numEnemiesLeft;
    public List<Enemy> listEnemiesLeft; // data of remaining enemies
    // public List<buffs?> buffStructures;  // buffs characters are taking into battle (no type for soup yet?)

    [Header("Dialog")]
    public string gamePhase; // GamePhase: What “day” it is.
    public int dayNum;       // DayNum: The number of days spent on the current Phase
    
    [Header("Soup")]
    public List<Ingredient> gatheredIngredients;  // List of gathered ingredients
    // public List<Soup> soupPool;                   // Soup pool (no type for soup yet?)

    private void Awake()
    {
        // initialize variables
        wounds = new Dictionary<string, int>();
        listEnemiesLeft = new List<Enemy>();
        // buffStructures = new List<
        gatheredIngredients = new List<Ingredient>();
        // soupPool = new List<Soup
    }
}
