using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public Cursor cursor;
    public List<PartyMember> party;
    private List<PartyMember> turnAvailible = new List<PartyMember>();
    private int selected;
    private bool inMenu = false;

    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Debug.Log("Party Phase Starting");
        turnAvailible.AddRange(party);
        return null;
    }

    public override void OnPhaseUpdate()
    {
        if (inMenu)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (++selected >= turnAvailible.Count)
                selected = 0;
            cursor.Select(turnAvailible[selected]);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (--selected < 0)
                selected = turnAvailible.Count - 1;
            cursor.Select(turnAvailible[selected]);
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            var highlighted = BattleGrid.main.GetObject(cursor.Row, cursor.Col);
            if(highlighted != null)
            {
                inMenu = highlighted.Select();
            }
        }
        else
        {
            cursor.OnPhaseUpdate();
        }       
    }

    public void EndMenu(PartyMember turnTaken)
    {
        if (turnTaken != null)
            turnAvailible.Remove(turnTaken);
        if (turnAvailible.Count <= 0)
            PhaseManager.main.NextPhase();
        inMenu = false;
    }
}
