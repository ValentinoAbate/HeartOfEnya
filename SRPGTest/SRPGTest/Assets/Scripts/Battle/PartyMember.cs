using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant
{
    public ActionMenu ActionMenu;
    public bool HasTurn { get; set; } = false;
    private MoveCursor cursor;

    protected override void Initialize()
    {
        base.Initialize();
        cursor = GetComponent<MoveCursor>();
        PhaseManager.main?.PartyPhase.Party.Add(this);
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
    }

    public void CloseActionMenu()
    {
        ActionMenu.SetActive(false);
    }

    public void CancelActionMenu()
    {
        ActionMenu.gameObject.SetActive(false);
        cursor.ResetToLastPosition();
        cursor.SetActive(true);
    }
}
