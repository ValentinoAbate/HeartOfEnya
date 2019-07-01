using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combatant : FieldObject
{
    public System.Func<Pos, Coroutine> preparedAction;
    public TargetPattern preparedTarget;
    public int maxHp;  
    public int atk;
    public int def;    
    public int mag;
    public int move;
    public int cook;

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
    private int armor;
    public int Armor
    {
        get => armor;
        set
        {
            armor = value;
            armorText.text = armor.ToString();
        }
    }

    public Text hpText;
    public Text armorText;

    protected override void Initialize()
    {
        base.Initialize();
        Hp = maxHp;
        Armor = def;       
    }
    public void Damage(int damage)
    {
        if(armor > 0)
        {
            if (damage > armor)
                Hp = Mathf.Max(0, hp - (damage - armor));
            Armor = Mathf.Max(0, armor - damage);
        }
        else
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
        Armor = def;
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
