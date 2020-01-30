using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A combatant is any FieldObject that is can use Actions (see Action.cs) or be affected by them.
/// Combatants have a movement stat, an HP stat, elemental reactions, and can be Stunned.
/// </summary>
public abstract class Combatant : FieldObject
{
    public abstract Sprite DisplaySprite { get; }
    public abstract Color DisplaySpriteColor { get; }
    [Header("Cheats")]
    [SerializeField]
    public bool godMode = false;
    [Header("Combatant Display Fields")]
    [TextArea(1, 2)]
    public string description = "Combatant description";

    [Header("General Combatant Fields")]
    public int maxHp;
    /// <summary>
    /// The units current movement range. Is only 1 square when an action is being charged.
    /// </summary>
    public int Move => IsChargingAction ? 1 : move;
    [SerializeField]
    private int move;

    #region Elemental Reactions

    public ActionEffect.Reaction reactionToPhys;
    public ActionEffect.Reaction reactionToMagic;
    public ActionEffect.Reaction reactionToFire;
    public ActionEffect.Reaction reactionToIce;
    public ActionEffect.Reaction reactionToSuppport;
    /// <summary>
    /// The action dictionary to look up reactions by an element as a key.
    /// Initialized in Initialize()
    /// </summary>
    [System.NonSerialized]
    public ActionEffect.ReactionDict reactions;

    #endregion
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
                hpImage.color = Color.yellow;
                CancelChargingAction();
            }
            else
            {
                Debug.Log(name + " is no longer stunned");
                hpImage.color = unstunnedColor;
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
    private int hp;
    [Header("UI References")]
    public Text hpText;
    public Image hpImage;
    public GameObject chargeUI;
    public Text chargeText;
    public bool IsChargingAction => chargingAction != null;
    public bool ChargingActionReady => IsChargingAction && chargingAction.Ready;
    private ChargingAction chargingAction;

    protected override void Initialize()
    {
        base.Initialize();
        reactions = new ActionEffect.ReactionDict()
        {
            { ActionEffect.Attribute.Physical, reactionToPhys      },
            { ActionEffect.Attribute.Magic,    reactionToMagic     },
            { ActionEffect.Attribute.Fire,     reactionToFire      },
            { ActionEffect.Attribute.Ice,      reactionToIce       },
            { ActionEffect.Attribute.Support,  reactionToSuppport  }
        };
        Hp = maxHp; 
    }

    /// <summary>
    /// Damage this unit. Calls the Kill() method if Hp is 0
    /// </summary>
    public virtual void Damage(int damage)
    {
        Hp = Mathf.Max(0, hp - damage);
        if (Dead && !godMode)
        {
            Kill();
        }else if (godMode)
        {
            Immortal();
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
    /// Implements the ability for characters to not die when hp is 0
    /// </summary>
    public virtual void Immortal()
    {
        Debug.Log(name + "'s god mode is unlocked.");      
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
            ChargeChargingAction();
            chargeUI.SetActive(true);
        }
        return null;
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
    }

    /// <summary>
    /// Charged a charging action once. Updates Charge UI.
    /// </summary>
    public void ChargeChargingAction()
    {
        if (!IsChargingAction)
            return;
        chargingAction.Charge();
        chargeText.text = (chargingAction.TurnsLeft + 1).ToString();
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
        return cr;
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
