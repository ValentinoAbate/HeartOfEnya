using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant, IPausable
{
    public PauseHandle PauseHandle { get; set; }
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
    public AttackCursor attackCursor;
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
    private MoveCursor moveCursor;

    protected override void Initialize()
    {
        base.Initialize();
        moveCursor = GetComponent<MoveCursor>();
        PhaseManager.main?.PartyPhase.Party.Add(this);
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Add(this);
        Fp = maxFp;
        PauseHandle = new PauseHandle(null, moveCursor, attackCursor, ActionMenu);
    }

    private void OnDestroy()
    {
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Remove(this);
    }

    public override bool Select()
    {
        if(HasTurn)
        {
            moveCursor.SetActive(true);
            return true;
        }
        return false;
    }

    public override Coroutine StartTurn()
    {
        moveCursor.SetActive(true);
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
        moveCursor.ResetToLastPosition();
        moveCursor.SetActive(true);
    }

    public override void OnPhaseStart()
    {
        HasTurn = !Stunned;
        if (HasTurn && !IsChargingAction && Fp < maxFp)
            ++Fp;
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
        moveCursor.CalculateTraversable();
        moveCursor.DisplayTraversable(true);
    }

    public override void UnHighlight()
    {
        moveCursor.DisplayTraversable(false);
    }

    public void Run()
    {
        EndAction();
        Destroy(gameObject);
    }

    public override void UseAction(Action action, Pos targetPos)
    {
        base.UseAction(action, targetPos);
        var fpCost = action.GetComponent<ActionFpCost>();
        if(fpCost != null)
            Fp -= fpCost.fpCost;
    }
}
