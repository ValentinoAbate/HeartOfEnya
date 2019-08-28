using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[DisallowMultipleComponent]
public class ActionMenu : MonoBehaviour
{
    public PartyMember user;
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
                var conditions = button.GetComponents<ActionCondition>();
                // No conditions (button should be active)
                if (conditions.Length <= 0)
                {
                    button.gameObject.SetActive(true);
                    continue;
                }
                // If the button fails any of the conditions, disable it
                button.gameObject.SetActive(conditions.All((c) => c.CheckCondition(user)));
            }
            gameObject.SetActive(true);
            var first = buttons.FirstOrDefault((b) => b.gameObject.activeSelf);
            first?.Select();
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }
}
