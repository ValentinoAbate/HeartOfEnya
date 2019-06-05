using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant
{
    public GameObject ActionMenu;
    public UnityEngine.UI.Button FirstButton;
    public bool HasTurn { get; set; } = false;
    private MoveCursor cursor;

    private void Start()
    {
        cursor = GetComponent<MoveCursor>();
        Initialize();
    }

    public override bool Select()
    {
        if(HasTurn)
        {
            cursor.SetActive(true);
            return true;
        }
        return false;
    }

    public override void OnPhaseStart()
    {
        base.OnPhaseStart();
        HasTurn = true;
    }

    public void EndAction()
    {
        var phase = PhaseManager.main.ActivePhase as PartyPhase;
        phase.EndAction(this);
    }

    public void OpenActionMenu()
    {
        ActionMenu.SetActive(true);
        FirstButton.Select();
    }

    public void CloseActionMenu()
    {
        ActionMenu.gameObject.SetActive(false);
    }

    public void CancelActionMenu()
    {
        ActionMenu.gameObject.SetActive(false);
        cursor.ResetToLastPosition();
        cursor.SetActive(true);
    }
}
