using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
	public Button button; //reference to the button object this is attached to

    // disable the button if there isn't a save file
    void Start()
    {
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "SaveData.txt"); //naughty ben, hardcoding filenames! Should probably fix later

        //abort if the save file and/or directory doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Debug.Log("No save file detected - disabling load button");
            button.interactable = false;
            return;
        }
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file detected - disabling load button");
            button.interactable = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
