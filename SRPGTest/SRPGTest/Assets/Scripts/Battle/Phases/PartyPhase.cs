using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public SelectNextCursor cursor;
    public List<PartyMember> party;
    private int selected;

    public override Coroutine OnPhaseEnd()
    {
        cursor.gameObject.SetActive(false);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        party.RemoveAll((e) => e == null);
        Debug.Log("Party Phase Starting");
        cursor.SelectionList.Clear();
        cursor.SelectionList.AddRange(party);
        party.ForEach((p) => p.OnPhaseStart() );
        cursor.HighlightFirst();
        cursor.SetActive(true);
        return null;
    }

    public void EndAction(PartyMember p)
    {
        cursor.SelectionList.Remove(p);
        if (cursor.SelectionList.Count <= 0)
            PhaseManager.main.NextPhase();
        else
        {
            cursor.HighlightNext();
            cursor.SetActive(true);
        }
    }

    public override void OnPhaseUpdate() { }
}
