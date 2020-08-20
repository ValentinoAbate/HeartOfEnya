using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConfirmationMenu : MonoBehaviour
{
    //make sure we're disabled on initial startup
    void Start()
    {
        gameObject.SetActive(false);
    }

    //Called when the user clicks on the "New Game" button.
    //If a save file exists, brings up the confirmation menu to ask if the player wants to overwrite their save.
    //If no save file exists, launches the first scene (i.e. mimics the old behavior of the button)
    public void activateMenu(string sceneName)
    {
        if (doesSaveExist())
        {
            Debug.Log("Asking for permission to wipe save file...");
            gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("No save file exists - skipping confirmation popup");
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            pData.returnScene = "IntroCamp"; //mark what scene to return to on game load
            pData.SaveToFile(); //Save new file
            SceneTransitionManager.main.TransitionScenes(sceneName);
        }
    }

    //Disables the menu if/when the user clicks "no" in the popup menu.
    public void deactivateMenu()
    {
    	gameObject.SetActive(false);
    }

    //utility method to check whether a save file exists
    private bool doesSaveExist()
    {
        //get path to the save file
        string savePath = Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "SaveData.txt"); //naughty ben, hardcoding filenames! Should probably fix later

        //return false if the save file and/or directory doesn't exist
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            return false;
        }
        if (!File.Exists(savePath))
        {
            return false;
        }

        //the save file exists - return true
        return true;
    }
}
