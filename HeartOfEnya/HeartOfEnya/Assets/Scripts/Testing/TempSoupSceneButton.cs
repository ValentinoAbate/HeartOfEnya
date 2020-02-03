using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempSoupSceneButton : MonoBehaviour
{
	public GameObject soupType; //what type of soup to spawn

	//change button text when clicked
    public void SetText(string text)
    {
    	Text txt = transform.Find("Text").GetComponent<Text>();
    	txt.text = text;
    }

    //spawn the soup at the center of the screen
    public void SpawnSoup()
    {
    	var soup = Instantiate(soupType, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void changeScene(string sceneName)
    {
        SceneTransitionManager.main.TransitionScenes(sceneName);
    }
}
