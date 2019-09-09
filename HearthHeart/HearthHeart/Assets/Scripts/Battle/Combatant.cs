using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combatant : FieldObject
{
    [Header("General Combatant Fields")]
    public int maxHp;
    public int Move => IsChargingAction ? 1 : move;
    [SerializeField]
    private int move;
    public ActionEffect.Reaction reactionToPhys;
    public ActionEffect.Reaction reactionToMagic;
    public ActionEffect.Reaction reactionToFire;
    public ActionEffect.Reaction reactionToIce;
    [System.NonSerialized]
    public ActionEffect.ReactionDict reactions;
    public virtual bool Stunned
    {
        get => stunned;
        set
        {
            stunned = value;
            if(value)
            {
                Debug.Log(name + " is stunned!");
                hpImage.color = Color.yellow;
                CancelChargingAction();
            }
            else
            {
                Debug.Log(name + " is no longer stunned");
                hpImage.color = Color.red;
            }
        }
    }
    protected bool stunned;
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
            { ActionEffect.Attribute.Physical, reactionToPhys  },
            { ActionEffect.Attribute.Magic,    reactionToMagic },
            { ActionEffect.Attribute.Fire,     reactionToFire  },
            { ActionEffect.Attribute.Ice,      reactionToIce   },
        };
        Hp = maxHp; 
    }

    public void Damage(int damage)
    {
        Hp = Mathf.Max(0, hp - damage);
        if (Dead)
            Kill();
    }
    public virtual void Kill()
    {
        chargingAction?.Cancel();
        Debug.Log(name + " has died...");
        Destroy(gameObject);
    }

    public virtual void UseAction(Action action, Pos targetPos)
    {
        if (action.chargeTurns <= 0)
        {
            var actionClone = Instantiate(action.gameObject).GetComponent<Action>();
            actionClone.Activate(this, targetPos);
        }
        else
        {
            chargingAction = new ChargingAction(action, this, targetPos);            
            ChargeChargingAction();
            chargeUI.SetActive(true);
        }
    }

    #region Action Charging
    public void CancelChargingAction()
    {
        if (!IsChargingAction)
            return;
        chargeUI.SetActive(false);
        chargingAction.Cancel();
        chargingAction = null;
    }

    public void ChargeChargingAction()
    {
        if (!IsChargingAction)
            return;
        chargingAction.Charge();
        chargeText.text = chargingAction.TurnsLeft.ToString();
    }

    public void ActivateChargedAction()
    {
        if (!IsChargingAction)
            return;
        Debug.Log(name + " uses charged action!");
        chargeUI.SetActive(false);
        chargingAction.Activate();
        chargingAction = null;
    }
    #endregion
}
