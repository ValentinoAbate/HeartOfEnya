using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveNewGameButton : MonoBehaviour
{
    public void Save()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.returnScene = "IntroCamp"; //mark what scene to return to on game load
        pData.SaveToFile(); //Save new file
    }
}
