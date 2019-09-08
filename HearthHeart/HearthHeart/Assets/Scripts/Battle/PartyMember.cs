using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant
{
    public override bool Stunned
    {
        get => stunned;
        set
        {
            base.Stunned = value;
            if (value)
            {
                if (HasTurn)
                    HasTurn = false;
            }
        }
    }
    public override Teams Team => Teams.Party;
    public int Fp
    {
        get => fp;
        set
        {
            fp = value;
            fpText.text = fp.ToString();
        }
    }
    private int fp;
    public Text fpText;
    [Header("Party Member Specific Fields")]
    public ActionMenu ActionMenu;
    public int maxFp;
    public int level;

    public bool HasTurn
    {
        get => hasTurn;
        set
        {
            hasTurn = value;
            var spr = GetComponent<SpriteRenderer>();
            if (spr == null)
                return;
            if (value)
            {
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
            }
            else
            {
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.5f);
            }
        }
    }
    private bool hasTurn = false;
    private MoveCursor cursor;

    protected override void Initialize()
    {
        base.Initialize();
        cursor = GetComponent<MoveCursor>();
        PhaseManager.main?.PartyPhase.Party.Add(this);
        Fp = maxFp;
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

    public override Coroutine StartTurn()
    {
        cursor.SetActive(true);
        return null;
    }

    public void EndAction()
    {
        var phase = PhaseManager.main.ActivePhase as PartyPhase;
        HasTurn = false;
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

    public override void OnPhaseStart()
    {
        if (IsChargingAction && !ChargingActionReady)
        {
            ChargeChargingAction();
            HasTurn = false;
            return;
        }
        HasTurn = !Stunned;
    }

    public override void OnPhaseEnd()
    {
        var spr = GetComponent<SpriteRenderer>();
        if (spr != null)
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1);
        if(Stunned)
            Stunned = false;
    }

    public override void Highlight()
    {
        cursor.CalculateTraversable();
        cursor.DisplayTraversable(true);
    }

    public override void UnHighlight()
    {
        cursor.DisplayTraversable(false);
    }

    public void Run()
    {
        EndAction();
        Destroy(gameObject);
    }
}
