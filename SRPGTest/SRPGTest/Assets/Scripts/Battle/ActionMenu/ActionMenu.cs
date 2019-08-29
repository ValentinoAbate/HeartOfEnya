using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[DisallowMultipleComponent]
public class ActionMenu : MonoBehaviour
{
    public enum SpecialAction
    {
        Run,
    }

    public PartyMember user;
    private List<Button> buttons = null;
    private HashSet<SpecialAction> specialActionsEnabled = new HashSet<SpecialAction>();

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

    public bool IsSpecialActionEnabled(SpecialAction action)
    {
        return specialActionsEnabled.Contains(action);
    }

    public void EnableSpecialAction(SpecialAction action)
    {
        if(!specialActionsEnabled.Contains(action))
            specialActionsEnabled.Add(action);
    }

    public void DisableSpecialAction(SpecialAction action)
    {
        if (specialActionsEnabled.Contains(action))
            specialActionsEnabled.Remove(action);
    }
}
