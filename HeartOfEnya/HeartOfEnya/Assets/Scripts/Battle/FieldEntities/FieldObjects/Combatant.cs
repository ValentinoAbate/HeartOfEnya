using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A combatant is any FieldObject that is can use Actions (see Action.cs) or be affected by them.
/// Combatants have a movement stat, an HP stat, elemental reactions, and can be Stunned.
/// </summary>
public abstract class Combatant : FieldObject
{
    public const bool stunIsBurn = true;
    public abstract Sprite DisplaySprite { get; }
    public abstract Color DisplaySpriteColor { get; }
    [Header("Cheats")]

    [Header("Combatant Display Fields")]
    [TextArea(1, 2)]
    public string description = "Combatant description";
    public AudioClip damageSfx;
    public AudioClip deathSfx;
    public AudioClip moveSfx;
    [Header("General Combatant Fields")]
    public int maxHp;
    /// <summary>
    /// The units current movement range. Is only 1 square when an action is being charged.
    /// </summary>
    public int Move => IsChargingAction ? 1 : move;
    [SerializeField]
    private int move;

    /// <summary>
    /// Is this unit stunned?
    /// Setting this value will automatically update the unit's UI
    /// </summary>
    public virtual bool Stunned
    {
        get => stunned;
        set
        {
            stunned = value;
            if(value)
            {
                Debug.Log(name + " is stunned!");
                unstunnedColor = hpImage.color;
                hpImage.color = new Color(1, 0.55f, 0.55f, 1);
                uiHelper.SetStun(true);
                CancelChargingAction();
            }
            else
            {
                Debug.Log(name + " is no longer stunned");
                hpImage.color = unstunnedColor;
                uiHelper.SetStun(false);
            }
        }
    }
    protected bool stunned;
    // The color to set the hp image back to after unstunning
    private Color unstunnedColor;
    /// <summary>
    /// Is this unit dead? (true if Hp <= 0)
    /// </summary>
    public bool Dead { get => Hp <= 0; }  
    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            hpText.text = hp.ToString();
        }
    }
    private int hp = 0;
    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public Image hpImage;
    public GameObject chargeUI;
    public TextMeshProUGUI chargeText;
    public UnitUI uiHelper;
    public bool IsChargingAction => chargingAction != null;
    private ChargingAction chargingAction;

    protected override void Initialize()
    {
        base.Initialize();
        if(Hp <= 0)
            Hp = maxHp; 
    }

    /// <summary>
    /// Damage this unit. Calls the Kill() method if Hp is 0
    /// </summary>
    public virtual void Damage(int damage)
    {
        if (damage > 0)
        {
            Hp = Mathf.Max(0, hp - damage);
            if (Dead)
            {
                Kill();
            }
        }
    }

    /// <summary>
    /// Cancel the charging action if applicable, and then destroy the object
    /// </summary>
    public virtual void Kill()
    {
        chargingAction?.Cancel();
        Debug.Log(name + " has died...");
        Destroy(gameObject);
    }

    /// <summary>
    /// Have the unit use an action at a certain target position
    /// </summary>
    /// <param name="action"></param>
    /// <param name="targetPos"></param>
    public virtual Coroutine UseAction(Action action, Pos targetPos)
    {
        // If the action is not a charged action
        if (action.chargeTurns <= 0)
        {
            // Instantiate a copy of the action object
            var actionClone = Instantiate(action.gameObject).GetComponent<Action>();
            return StartCoroutine(actionClone.Activate(this, targetPos));
        }
        else // The action is a charged action, start charging
        {
            chargingAction = new ChargingAction(action, this, targetPos);            
            //chargeUI.SetActive(true);
            uiHelper.SetCharge(true);
        }
        return null;
    }

    public override void OnPhaseEnd()
    {
        if (Dead)
            return;
        if(Stunned)
        {
            Stunned = false;
            if(stunIsBurn)
                Damage(1);
        }
    }

    #region Action Charging

    /// <summary>
    /// Cancel a charged action without using it. Updates Charge UI.
    /// </summary>
    public void CancelChargingAction()
    {
        if (!IsChargingAction)
            return;
        chargeUI.SetActive(false);
        chargingAction.Cancel();
        chargingAction = null;
        uiHelper.SetCharge(false);
    }

    /// <summary>
    /// Active a charged action. Updates Charge UI.
    /// The action being fully charged is not a prerequisite of this function working,
    /// But should likely be true.
    /// </summary>
    public Coroutine ActivateChargedAction()
    {
        if (!IsChargingAction)
            return null;
        Debug.Log(name + " uses charged action!");
        chargeUI.SetActive(false);
        var cr = StartCoroutine(chargingAction.Activate());
        chargingAction = null;
        uiHelper.SetCharge(false);
        return cr;
    }

    public void UpdateChargeTileUI()
    {
        if (IsChargingAction)
            chargingAction.UpdateDisplay();
    }

    public void UpdateChargeTileUI(Pos p)
    {
        if (IsChargingAction)
            chargingAction.UpdateDisplay(p);
    }

    #endregion

    public override void Highlight()
    {
        BattleUI.main.ShowInfoPanelGeneric(this);
    }

    public override void UnHighlight()
    {
        BattleUI.main.HideInfoPanel();
    }
}
