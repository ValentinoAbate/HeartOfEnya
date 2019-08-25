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
    public ActiveAbilityEffect.ReactionDict reactions = new ActiveAbilityEffect.ReactionDict();

    public bool Dead { get => Hp == 0; }
    private int hp;
    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            hpText.text = hp.ToString();
        }
    }
    public Text hpText;

    protected override void Initialize()
    {
        base.Initialize();
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
