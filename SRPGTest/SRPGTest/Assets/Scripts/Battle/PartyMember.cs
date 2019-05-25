using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : Combatant
{
    public GameObject ActionMenu;
    public UnityEngine.UI.Button FirstButton;

    public override bool Select()
    {
        ActionMenu.SetActive(true);
        FirstButton.Select();
        return true;
    }
}
