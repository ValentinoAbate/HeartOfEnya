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
    public KeyCode cancelKey;
    public bool allowFlameMode;
    public bool FlameMode { get; set; }
    private List<Button> buttons = null;
    private HashSet<SpecialAction> specialActionsEnabled = new HashSet<SpecialAction>();

    private void Update()
    {
        if (Input.GetKeyDown(cancelKey))
        {
            if(buttons.Count > 0)
                buttons[buttons.Count - 1].Select();
            user.CancelActionMenu();
        }
            
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
            FlameMode = false;
            InitializeMenu();
        }            
        else
            gameObject.SetActive(false);      
    }

    public void InitializeMenu(Button select = null)
    {
        foreach (var button in buttons)
        {
            var conditions = button.GetComponents<ActionCondition>();
            // No conditions (button should be active)
            if (conditions.Length <= 0)
            {
                button.gameObject.SetActive(true);
                button.interactable = true;
                continue;
            }
            // Find all the failed conditions
            var failedConditions = conditions.Where((c) => !c.CheckCondition(this, user));
            // If there are none, set the button to active and interactable
            if (failedConditions.Count() <= 0)
            {
                button.gameObject.SetActive(true);
                button.interactable = true;
            }
            // If a failed condition should hide the button, set it to be inactive
            else if (failedConditions.Any((c) => c.onConditionFail == ActionCondition.OnConditionFail.Hide))
            {
                button.gameObject.SetActive(false);
            }
            // Else, the button has failed a condition that should just make in uninteractable
            // Set it to be active and disable interaction
            else
            {
                button.gameObject.SetActive(true);
                button.interactable = false;
            }
        }
        // Enable the action menu
        gameObject.SetActive(true);
        // Select the argument button if there is one
        if (select != null)
            select.Select();
        else
        {
            // Select the first active button
            var first = buttons.FirstOrDefault((b) => b.gameObject.activeSelf);
            first?.Select();
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
