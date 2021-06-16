﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// A Combatant that is a member of the party.
/// Adds the Fp stat, along with functionality to track and display turn status and active action menus.
/// </summary>
[RequireComponent(typeof(MoveCursor))]
public class PartyMember : Combatant, IPausable
{
    public const int maxDeathsDoorCounter = 5;
    public const int minLevel = 1;
    public override bool Stunned
    {
        get => stunned;
        set
        {
            base.Stunned = value;
            // Remove the ability to take a turn if just stunned and the unit still had a turn
            if (value && HasTurn)
            {
                HasTurn = false;
            }
        }
    }
    // Party members are always on the Party team
    public override Teams Team => Teams.Party;
    /// <summary>
    /// The number of Fp (Flame Points) the unit currently has
    /// Setting this number will automatically update the Fp UI.
    /// </summary>
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
    public TextMeshProUGUI fpText;
    public GameObject deathsDoorUI;
    public TextMeshProUGUI deathsDoorCounterText;
    public GameObject fpImage;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private Character chara;
    [Header("Party Member Specific Fields")]
    [SerializeField] private ActionMenu actionMenuRight;
    [SerializeField] private ActionMenu actionMenuLeft;
    public AttackCursor attackCursor;
    public int maxFp;
    public int level;
    public Color noTurnColor = Color.gray;
    public override Sprite DisplaySprite => chara.Portrait;
    public override Color DisplaySpriteColor => Color.white;
    private FMODUnity.StudioEventEmitter sfxHighlight;
    private PlaytestLogger logger;

    public bool DeathsDoor { get; private set; }
    public ActionMenu ActionMenu
    {
        get
        {
            if (Pos.col == BattleGrid.main.Cols - 1 
                || (Pos.col == BattleGrid.main.Cols - 2 && Pos.row >= BattleGrid.main.Rows - 2))
            {
                return actionMenuLeft;
            }
            return actionMenuRight;
        }
    }

    public int DeathsDoorCounter
    {
        get => deathsDoorCounter;
        set
        {
            deathsDoorCounter = Mathf.Max(0,value);
            deathsDoorCounterText.text = deathsDoorCounter.ToString();
            if (deathsDoorCounter == 1)
                deathsDoorUI.GetComponent<Image>().color = Color.red;
            else if (deathsDoorCounter <= 0)
                Kill();
        }
    }
    private int deathsDoorCounter;

    /// <summary>
    /// Can this unit still take an action this turn?
    /// Updates the UI when set. Current effect just makes the unit semi-transparent.
    /// </summary>
    public bool HasTurn
    {
        get => hasTurn;
        set
        {
            hasTurn = value;
            if (value && !disabled)
            {
                sprite.color = Color.white;
            }
            else
            {
                sprite.color = noTurnColor;
            }
        }
    }
    private bool hasTurn = false;
    public bool Disabled
    {
        get => disabled;
        set
        {
            disabled = value;
            if (!value)
            {
                sprite.color = Color.white;
            }
            else
            {
                sprite.color = noTurnColor;
            }
        }
    }
    private bool disabled = false;

    public bool RanAway { get; private set; }

    private MoveCursor moveCursor;
    private MouseMoveCursor mouseMoveCursor;

    protected override void Initialize()
    {
        // Initialize base combatant logic
        base.Initialize();
        moveCursor = GetComponent<MoveCursor>();
        mouseMoveCursor = GetComponent<MouseMoveCursor>();
        // Add this unit to the party phase
        PhaseManager.main?.PartyPhase.Party.Add(this);
        // Add this unit to the party phase's pause dependents, so this unit is paused when the party phase is paused
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Add(this);
        Fp = maxFp;
        DeathsDoorCounter = maxDeathsDoorCounter;
        // Initialize the pause handle with the cursors and action menu as dependents
        PauseHandle = new PauseHandle(null, moveCursor, mouseMoveCursor, attackCursor, actionMenuRight, actionMenuLeft);

        // Find reference to FMOD event emitter
        FMODBattle.main.Music.SetParameter("Loading", 0);

        logger = DoNotDestroyOnLoad.Instance.playtestLogger;
        EnableFP();
    }

    public string GetName()
    {
        return chara.Name;
    }

    public override void Damage(int damage)
    {
        if (passiveAbility == Passives.Sturdy)
            --damage;
        // log hp change in playtest logger
        logger.testData.hp[GetName()] += damage;
        if (damage > 0)
        {
            Hp = Mathf.Max(0, Hp - damage);
            if (Dead && !DeathsDoor)
                EnterDeathsDoor();
            else // Decrement counter if in deaths
            {
                if(DeathsDoor)
                    --DeathsDoorCounter;
                if (damageFxPrefab != null)
                    Instantiate(damageFxPrefab, VfxSpawnPoint, Quaternion.identity).GetComponent<ActionVfx>()?.Play();
                damageSfxEvent.Play();
                animator.Play("Damage");
            }              
        }
    }

    /// <summary>
    /// Changes a character to death's door. adds FMOD integration to change
    /// the battle theme when a unit's HP reaches 0
    /// </summary>
    public void EnterDeathsDoor()
    {
        if (deathFxPrefab != null)
            Instantiate(deathFxPrefab, VfxSpawnPoint, Quaternion.identity).GetComponent<ActionVfx>()?.Play();
        deathSfxEvent.Play();
        DeathsDoor = true;
        Debug.Log(DisplayName + "Has Enetered Death's Door");
        FMODBattle.main.EnterCrisis(this);
        deathsDoorUI.SetActive(true);
        hpImage.gameObject.SetActive(false);
        fpText.transform.parent.gameObject.SetActive(false);
        CancelChargingAction();

        // invoke tutorial trigger for death's door
        BattleEvents.main.tutDD._event.Invoke();
    }

    public override void Kill()
    {
        Debug.Log(DisplayName + " has died");
        BattleEvents.main.graveInjury._event.Invoke();
    }

    private void OnDestroy()
    {
        PhaseManager.main?.PartyPhase.PauseHandle.Dependents.Remove(this);
    }

    /// <summary>
    /// Start the unit's turn by activating the Movecursor
    /// If HasTurn == false, do nothing
    /// </summary>
    /// <returns> HasTurn </returns>
    public override bool Select()
    {
        // make unit unselectable if the tutorial disables them
        if(HasTurn && !disabled)
        {
            // moveCursor.SetActive(true);
            mouseMoveCursor.SetActive(true);
            BattleUI.main.HideEndTurnButton();

            // run tutorial trigger when raina is selected
            if(GetName() == "Raina")
            {
                BattleEvents.main.tutMove._event.Invoke();
            }
            // run tutorial trigger when soleil is selected on turn 2
            else if(GetName() == "Soleil" && PhaseManager.main.Turn == 2)
            {
                BattleEvents.main.tutSoleilChargeExplanation._event.Invoke();
            }
            // run tutorial trigger when lua is selected
            else if(GetName() == "Lua")
            {
                BattleEvents.main.tutLua._event.Invoke();
            }

            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Fully end the unit's turn after taking an action.
    /// </summary>
    public void EndTurn()
    {
        BattleUI.main.ShowEnemiesRemaining();
        HasTurn = false;
        PhaseManager.main.PartyPhase.EndAction(this);
    }

    public void OpenActionMenu(bool skipOneCancel = false)
    {
        if (Pos.row == 0 && Pos.col == 0 || Pos.row ==0 && Pos.col == 1)
        {
            BattleUI.main.HideEnemiesRemaining();
        }
        if (skipOneCancel)
            ActionMenu.SetSkipOneCancel();
        ActionMenu.SetActive(true);

        // cancel trigger for bapy
        if(GetName() == "Bapy")
        {
            BattleEvents.main.tutBapyCancel._event.Invoke();
        }
    }

    /// <summary>
    /// Close the action menu and return to the movement phase of a turn.
    /// Should only be called when the action menu is open
    /// </summary>
    public void CancelActionMenu()
    {
        BattleUI.main.ShowEnemiesRemaining();
        ActionMenu.gameObject.SetActive(false);
        // moveCursor.ResetToLastPosition();
        // moveCursor.SetActive(true);
        mouseMoveCursor.ResetToLastPosition();
        mouseMoveCursor.SetActive(true);
    }

    public void ClearSoloActions()
    {
        actionMenuLeft.ClearSoloActions();
        actionMenuRight.ClearSoloActions();
    }

    public void SoloAction(string actionID)
    {
        actionMenuLeft.SoloAction(actionID);
        actionMenuRight.SoloAction(actionID);
    }

    public void UnSoloAction(string actionID)
    {
        actionMenuLeft.UnSoloAction(actionID);
        actionMenuRight.UnSoloAction(actionID);
    }

    new public void ActivateChargedAction()
    {
        StartCoroutine(ActivateChargedActionCr());
    }

    private IEnumerator ActivateChargedActionCr()
    {
        yield return base.ActivateChargedAction();
        EndTurn();
    }

    /// <summary>
    /// Allow the unit to take an action this turn (if not stunned)
    /// </summary>
    public override void OnPhaseStart()
    {
        HasTurn = !Stunned;
        //if (HasTurn && !IsChargingAction && Fp < maxFp)
        //    ++Fp;
    }

    public override void OnPhaseEnd()
    {
        sprite.color = Color.white;
        if(Stunned)
        {
            Stunned = false;
            if (stunIsBurn)
                Damage(1);
        }            
        if (DeathsDoor)
            --DeathsDoorCounter;
    }

    /// <summary>
    /// If the party member still take a turn this phase, calculate the movement range and display it.
    /// </summary>
    public override void Highlight()
    {
        if (stunned || !hasTurn || Disabled)
            return;
        // moveCursor.CalculateTraversable();
        // moveCursor.DisplayTraversable(true);
        // sfxHighlight.Play();
        mouseMoveCursor.CalculateTraversable();
        mouseMoveCursor.DisplayTraversable(true);
        BattleUI.main.ShowInfoPanelParty(this);
    }

    // Hide the movement range display
    public override void UnHighlight()
    {
        // moveCursor.DisplayTraversable(false);
        mouseMoveCursor.DisplayTraversable(false);
        BattleUI.main.HideInfoPanel();
    }

    /// <summary>
    /// Use the special action: run. Simply destroys the unit for now
    /// </summary>
    public void Run()
    {
        RanAway = true;

        FMODBattle.main.PartyMemberOutOfCrisis(this);
        // if there are no remaining party members in crisis
        if (DeathsDoor && PhaseManager.main.PartyPhase.Party.Count((p) => p.DeathsDoor && !p.RanAway) <= 0)
        {
            // but there are still party members on the field
            if (PhaseManager.main.PartyPhase.Party.Count((p) => p != null && !p.RanAway) >= 1)
            {
                FMODBattle.main.ExitCrisis();
            }
        }

        EndTurn();
        // Destroy(gameObject);
        gameObject.SetActive(false);
        BattleGrid.main.RemoveObject(this);
    }

    /// <summary>
    /// Use the action and reduce Fp if the action has an ActionFpCost component
    /// </summary>
    public override Coroutine UseAction(Action action, Pos targetPos, Pos primaryTarget)
    {
        BattleUI.main.ShowEnemiesRemaining();
        //Hide the info panel;
        BattleUI.main.HideInfoPanel();
        var routine = base.UseAction(action, targetPos, primaryTarget);
        var fpCost = action.GetComponent<ActionFpCost>();
        if(fpCost != null)
        {
            // log fp use in playtest logger
            logger.testData.fp[GetName()] += fpCost.fpCost;

            Fp -= fpCost.fpCost;
        }

        // log move use count and move damage in playtest logger
        // if move hasnt been used yet and isnt in dictionary,
        if(!logger.testData.moves.ContainsKey(action.name))
        {
            logger.testData.moves[action.name] = 1;

            // check if the move does damage
            var dmg = action.GetComponent<DamageEffect>();
            if(dmg != null)
            {
                logger.testData.moveDmg[action.name] = dmg.damage;
            }
        }
        else
        {
            logger.testData.moves[action.name] += 1;

            // check if the move does damage
            var dmg = action.GetComponent<DamageEffect>();
            if(dmg != null)
            {
                logger.testData.moveDmg[action.name] += dmg.damage;
            }
        }
            
        return routine;
    }

    // disables the unit so the player can't select them, used for tutorial scripting
    public void DisableUnit()
    {
        // might need to do more than this? not sure
        Disabled = true;
    }

    public void EnableUnit()
    {
        Disabled = false;
    }

    public void EnableFP()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.InTutorialFirstDay || pData.InTutorialSecondDay)
        {
            fpImage.SetActive(false); 
        }
        else
        {
            fpImage.SetActive(true);
        }
    }
}
