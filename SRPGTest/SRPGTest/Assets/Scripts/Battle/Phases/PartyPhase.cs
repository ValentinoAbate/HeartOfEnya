using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public Cursor cursor;
    public List<PartyMember> party;
    public List<PartyMember> turnAvailible;
    private int selected;

    public override Coroutine OnPhaseEnd()
    {
        cursor.gameObject.SetActive(false);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Debug.Log("Party Phase Starting");
        turnAvailible.AddRange(party);
        turnAvailible.ForEach((p) => p.HasTurn = true);
        selected = -1;
        SelectNext();
        cursor.SetActive(true);
        return null;
    }

    public void EndAction(PartyMember p)
    {
        turnAvailible.Remove(p);
        if (turnAvailible.Count <= 0)
            PhaseManager.main.NextPhase();
        SelectNext();
        cursor.SetActive(true);
    }

    public void SelectNext()
    {
        if (++selected >= turnAvailible.Count)
            selected = 0;
        var p = turnAvailible[selected];
        cursor.Highlight(p.Row, p.Col);
    }

    public void SelectPrev()
    {
        if (--selected < 0)
            selected = turnAvailible.Count - 1;
        var p = turnAvailible[selected];
        cursor.Highlight(p.Row, p.Col);
    }

    public override void OnPhaseUpdate()
    {
    }
}
