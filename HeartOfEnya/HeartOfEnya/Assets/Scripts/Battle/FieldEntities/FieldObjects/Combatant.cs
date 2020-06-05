using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

/// <summary>
/// A combatant is any FieldObject that is can use Actions (see Action.cs) or be affected by them.
/// Combatants have a movement stat, an HP stat, elemental reactions, and can be Stunned.
/// </summary>
public abstract class Combatant : FieldObject, IPausable
{
    public PauseHandle PauseHandle { get; set; }
    public enum Passives
    { 
        None,
        Sturdy,
        Small,
        Strong,
        Nimble,
    }

    public const bool stunIsBurn = true;
    public abstract Sprite DisplaySprite { get; }
    public abstract Color DisplaySpriteColor { get; }
    [Header("Passive Ability")]
    public Passives passiveAbility = Passives.None;

    [Header("Combatant Display Fields")]
    [TextArea(1, 2)]
    public string description = "Combatant description";
    public string passiveName = string.Empty;
    public string passiveDescription = string.Empty;
    public GameObject damageFxPrefab;
    public GameObject deathFxPrefab;
    // Debug Audio fields while we don't have FMOD setup
    public SfxPlayerDispatcher sfxDispatch;
    public AudioClip moveSfx;
    public StudioEventEmitter damageSfxEvent;
    public StudioEventEmitter deathSfxEvent;
    [Header("General Combatant Fields")]
    public bool invincible = false;
    public int maxHp;
    /// <summary>
    /// The units current movement range. Is only 1 square when an action is being charged.
    /// </summary>
    public int Move => IsChargingAction ? (passiveAbility == Passives.Nimble ? 2 : 1) : move;
    [SerializeField]
    private int move;
    public bool isMovable = true; //whether the combatant can be moved via MoveEffect

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
            animator.SetBool("Stunned", value);
            if (value)
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
    public Animator animator;
    public GameObject statsUI;
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
        if (passiveAbility == Passives.Sturdy)
            --damage;
        if (damage > 0)
        {
            Hp = Mathf.Max(0, hp - damage);
            if (Dead)
            {
                var pData = DoNotDestroyOnLoad.Instance.persistentData;
                if (!invincible)
                    Kill();
                else if(pData.InLuaBattle)
                {
                    BattleEvents.main.luaBossPhaseChange._event.Invoke();
                    BattleEvents.main.luaBossPhase2Defeated._event.Invoke();
                }
                else if(pData.gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle)
                {
                    BattleEvents.main.abs0PhaseChange._event.Invoke();
                    BattleEvents.main.abs0Phase2Defeated._event.Invoke();
                }
            }
            else
            {
                if(damageFxPrefab != null)
                    Instantiate(damageFxPrefab, VfxSpawnPoint, Quaternion.identity).GetComponent<ActionVfx>()?.Play();
                damageSfxEvent.Play();
                animator.Play("Damage");
            }
        }
    }

    /// <summary>
    /// Cancel the charging action if applicable, and then destroy the object
    /// </summary>
    public virtual void Kill()
    {
        CancelChargingAction();
        BattleGrid.main.RemoveObject(this);
        Debug.Log(name + " has died...");       
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        if(deathFxPrefab != null)
            Instantiate(deathFxPrefab, VfxSpawnPoint, Quaternion.identity).GetComponent<ActionVfx>()?.Play();
        deathSfxEvent.Play();
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    /// <summary>
    /// Have the unit use an action at a certain target position
    /// </summary>
    public virtual Coroutine UseAction(Action action, Pos targetPos, Pos primaryTarget)
    {
        // If the action is not a charged action
        if (action.chargeTurns <= 0)
        {
            // Instantiate a copy of the action object
            var actionClone = Instantiate(action.gameObject).GetComponent<Action>();
            return StartCoroutine(actionClone.Activate(this, targetPos, primaryTarget));
        }
        else // The action is a charged action, start charging
        {
            SetChargingAction(action, targetPos);
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
    /// Set the charging action and associated UI parameters
    /// </summary>
    private void SetChargingAction(Action action, Pos target)
    {
        chargingAction = new ChargingAction(action, this, target);
        animator.SetBool("Charging", true);
        //chargeUI.SetActive(true);
        uiHelper.SetCharge(true);

        // check if unit is an enemy
        if(Team == Teams.Enemy)
        {
            // run tutorial trigger for enemy charging action
            BattleEvents.main.tutEnemyRanged._event.Invoke();
        }
    }

    /// <summary>
    /// Clears the charging action properties. 
    /// called after activating or canceling a charged action
    /// </summary>
    private void ClearCharging()
    {
        animator.SetBool("Charging", false);
        chargingAction = null;
        uiHelper.SetCharge(false);
        chargeUI.SetActive(false);
    }

    /// <summary>
    /// Cancel a charged action without using it. Updates Charge UI.
    /// </summary>
    public void CancelChargingAction()
    {
        if (!IsChargingAction)
            return;
        chargingAction.Cancel();
        ClearCharging();
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
        ClearCharging();
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
