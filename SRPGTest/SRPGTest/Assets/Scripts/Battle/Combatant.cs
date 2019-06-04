using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combatant : FieldObject
{
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
            hpText.text = hp + " / " + maxHp;
        }
    }
    private int armor;
    public int Armor
    {
        get => armor;
        set
        {
            armor = value;
            armorText.text = armor + " / " + def;
        }
    }

    public Text hpText;
    public Text armorText;
    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
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
        Destroy(gameObject);
    }

    public override void OnPhaseStart()
    {
        Armor = def;
    }

}
