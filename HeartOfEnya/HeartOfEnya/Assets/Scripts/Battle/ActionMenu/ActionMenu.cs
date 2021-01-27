using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[DisallowMultipleComponent]
public class ActionMenu : MonoBehaviour, IPausable
{
    public enum SpecialAction
    {
        Run,
    }

    public PauseHandle PauseHandle { get; set; }
    public PartyMember user;
    public AttackCursor cursor;
    public KeyCode cancelKey;

    private List<Button> buttons = null;
    private HashSet<SpecialAction> specialActionsEnabled = new HashSet<SpecialAction>();
    private HashSet<string> actionsSoloed = new HashSet<string>();

    protected FMODUnity.StudioEventEmitter sfxCancel;

    public GameObject confirmationPrompt;
    private bool inConfirmationPrompt = false;

    public void SetSkipOneCancel() => skipOneCancel = true;
    private bool skipOneCancel = false;
 

    private void Awake()
    {
        PauseHandle = new PauseHandle(OnPause);
        
    }

    private void Start()
    {
        sfxCancel = GameObject.Find("UICancel").GetComponent<FMODUnity.StudioEventEmitter>();

    }

    private void Update()
    {
        if (PauseHandle.Paused || !BattleUI.main.CancelingEnabled || cursor.isActiveAndEnabled)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            if(skipOneCancel)
            {
                skipOneCancel = false;
                return;
            }
            Cancel();
        }
            
    }

    public void Cancel()
    {
        sfxCancel.Play();
        if (inConfirmationPrompt)
        {
            CancelConfirmationPrompt();
            return;
        }
        //if(buttons.Count > 0)
        //    buttons[buttons.Count - 1].Select();
        foreach (var button in buttons)
        {
            var actionComp = button.GetComponent<ActionButton>();
            actionComp?.HideExtraInfoWindow();
        }
        cursor.HideTargets();
        user.CancelActionMenu();
   
    }

    public void ShowConfirmationPrompt()
    {
        foreach (var button in buttons)
            button.interactable = false;
        confirmationPrompt.SetActive(true);
        inConfirmationPrompt = true;
    }

    public void CancelConfirmationPrompt()
    {
        HideConfirmationPrompt();
        InitializeMenu();
    }

    public void HideConfirmationPrompt()
    {
        confirmationPrompt.SetActive(false);
        inConfirmationPrompt = false;
    }

    private void OnPause(bool pause)
    {
        if (!isActiveAndEnabled)
            return;
        if (pause)
        {
            foreach (var button in buttons)
                button.interactable = false;
            if (inConfirmationPrompt)
                confirmationPrompt.SetActive(false);
        }
        else if (inConfirmationPrompt)
        {
            confirmationPrompt.SetActive(true);
        }
        else
        {
            InitializeMenu();
        }
    }

    private void FindButtons()
    {
        buttons = new List<Button>();
        buttons.AddRange(GetComponentsInChildren<Button>());
    }

    public void Close() => SetActive(false); 

    public void SetActive(bool value)
    {
        if (buttons == null)
            FindButtons();
        if (value)
        {
            InitializeMenu();
        }
        else
        {
            
            gameObject.SetActive(false);
        }
    }

    public void InitializeMenu(Button select = null)
    {
        foreach (var button in buttons)
        {
            var actionButton = button.GetComponent<ActionButtonBase>();
            if (actionsSoloed.Count > 0 && !actionsSoloed.Contains(actionButton.ID.ToLower()))
            {
                button.gameObject.SetActive(false);
                continue;
            }
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
        //if (select != null)
        //    select.Select();
        //else
        //{
        //    // Select the first active button
        //    var first = buttons.FirstOrDefault((b) => b.gameObject.activeSelf);
        //    first?.Select();
        //}
    }

    public void SoloAction(Action action) => SoloAction(action.ID);

    public void SoloAction(string actionID) => actionsSoloed.Add(actionID.ToLower());    
    
    public void UnSoloAction(Action action) => UnSoloAction(action.ID);

    public void UnSoloAction(string actionID) => actionsSoloed.Remove(actionID.ToLower());

    public void ClearSoloActions() => actionsSoloed.Clear();


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

    public void Wait()
    {
        user.EndTurn();
        // Hide any active tutorialization prompts
        BattleUI.main.HidePrompt();
        Close();
    }

    public void Run()
    {
        user.Run();
    }

    public void ActivateChargedAction()
    {
        user.ActivateChargedAction();
        Close();
    }

    public void ChargeChargedAction()
    {
        user.EndTurn();
        Close();
    }

    public void CancelChargedAction()
    {
        user.CancelChargingAction();
        user.EndTurn();
        Close();
    }
}
