using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPrototype : MonoBehaviour
{
    public GameObject ActionText;
    // Start is called before the first frame update
   public void DisplayText()
    {
        ActionText.SetActive(true);
    }
    public void HideText()
    {
        ActionText.SetActive(false);
    }
}
