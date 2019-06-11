using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPhase : Phase
{
    public SelectNextCursor cursor;
    private int selected;

    public List<PartyMember> Party { get; } = new List<PartyMember>();

    public override Coroutine OnPhaseEnd()
    {
        cursor.gameObject.SetActive(false);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        Party.RemoveAll((e) => e == null);
        cursor.SelectionList.Clear();
        cursor.SelectionList.AddRange(Party);
        Party.ForEach((p) => p.OnPhaseStart() );
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
