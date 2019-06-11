using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    private List<Button> buttons = null;

    private void Awake()
    {
        
    }

    private void FindButtons()
    {
        buttons = new List<Button>();
        buttons.AddRange(GetComponentsInChildren<Button>());
    }

    public void SetActive(bool value)
    {
        if (buttons == null)
            FindButtons();
        if (value)
        {          
            foreach(var button in buttons)
            {
                var condition = button.GetComponent<ActionCondition>();
                button.interactable = condition == null || condition.CheckCondition();
            }
            gameObject.SetActive(value);
            buttons[0].Select();
        }
        else
        {
            gameObject.SetActive(value);
        }
        
    }
}
