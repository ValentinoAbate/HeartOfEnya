using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combatant : FieldObject
{
    public System.Func<Pos, Coroutine> preparedAction;
    [System.NonSerialized]
    public TargetPattern preparedTarget;
    public int maxHp;  
    public int move;
    public ActiveAbilityEffect.Reaction reactionToPhys;
    public ActiveAbilityEffect.Reaction reactionToMagic;
    public ActiveAbilityEffect.Reaction reactionToFire;
    public ActiveAbilityEffect.Reaction reactionToIce;
    [System.NonSerialized]
    public ActiveAbilityEffect.ReactionDict reactions;
    public virtual bool Stunned
    {
        get => stunned;
        set
        {
            stunned = value;
            if(value)
            {
                CancelPreparedAction();
            }
        }
    }
    protected bool stunned;
    public bool Dead { get => Hp == 0; }  
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
    public Text hpText;

    protected override void Initialize()
    {
        base.Initialize();
        reactions = new ActiveAbilityEffect.ReactionDict()
        {
            { ActiveAbilityEffect.Attribute.Physical, reactionToPhys  },
            { ActiveAbilityEffect.Attribute.Magic,    reactionToMagic },
            { ActiveAbilityEffect.Attribute.Fire,     reactionToFire  },
            { ActiveAbilityEffect.Attribute.Ice,      reactionToIce   },
        };
        Hp = maxHp; 
    }

    public override bool CanMoveThrough(FieldObject other)
    {
        return other == null || (other is Combatant && other.Allegiance == Allegiance);
    }
    public void Damage(int damage)
    {
        Hp = Mathf.Max(0, hp - damage);
        if (Dead)
            Kill();
    }
    public virtual void Kill()
    {
        preparedTarget?.Hide();
        Destroy(gameObject);
    }

    public override Coroutine StartTurn()
    {
        if(preparedAction == null)
            return null;
        return StartCoroutine(DoPreparedAction());
    }
    protected void CancelPreparedAction()
    {
        if (preparedAction == null)
            return;
        preparedTarget.Hide();
        preparedTarget = null;
        preparedAction = null;
    }
    protected void PrepareAction(System.Func<Pos, Coroutine> action, TargetPattern targets)
    {
        preparedAction = action;
        preparedTarget = targets;
        preparedTarget.Show(BattleGrid.main.debugSquarePrefab);
    }
    protected IEnumerator DoPreparedAction()
    {
        preparedTarget.Hide();
        foreach(var pos in preparedTarget.Positions)
        {
            yield return preparedAction(pos);
        }
    }

    

}
