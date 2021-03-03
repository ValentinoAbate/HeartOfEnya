using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Literally based this off of Valentino's Player Singleton in Block 1
/// Essentially allows the transfer of data between scenes.
/// </summary>
public class DoNotDestroyOnLoad : MonoBehaviour
{
    private static DoNotDestroyOnLoad instance;
    public static DoNotDestroyOnLoad Instance { get { return instance; } }
    public PersistentData persistentData;
    public PlaytestLogger playtestLogger;
    public PersistentSaveData permanentSaveData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ResetPersistantData()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
            instance = null;
        }
    }
}
