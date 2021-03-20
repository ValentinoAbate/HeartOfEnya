using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainMenuSelector : MonoBehaviour
{
    private string ExistActionText => showIfFile ? "enabling" : "disabling";
    private string NotExistActionText => showIfFile ?  "disabling" : "enabling";
    public bool showIfFile = false;
    public PersistentData persistentData;
    // disable the button if there isn't a save file
    void Start()
    {
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, PersistentData.saveDataPath);
        savePath = Path.Combine(savePath, "SaveData.txt"); //naughty ben, hardcoding filenames! Should probably fix later

        //abort if the save file and/or directory doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Debug.Log("No save file detected - " + NotExistActionText + " " + name);
            gameObject.SetActive(!showIfFile);
            return;
        }
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file detected - " + NotExistActionText + " " + name);
            gameObject.SetActive(!showIfFile);
            return;
        }
        else
        {
            Debug.Log("Save file detected - " + ExistActionText + " " + name);
            persistentData.LoadFromFile();
            gameObject.SetActive(showIfFile ^ persistentData.gamePhase == PersistentData.gamePhaseGameComplete);
        }
    }
}
