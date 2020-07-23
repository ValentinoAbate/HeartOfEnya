using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTestSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	var pData = DoNotDestroyOnLoad.Instance.persistentData;
    	if(Input.GetKeyDown(KeyCode.Equals))//save
    	{
    		Debug.Log("Attempting save...");
    		pData.SaveToFile();
    	}
        else if(Input.GetKeyDown(KeyCode.Minus))//load
        {
            //DialogueManager.main.runner.Stop();
            Debug.Log("Attempting load...");
            pData.LoadFromFile();
        }
    }
}
