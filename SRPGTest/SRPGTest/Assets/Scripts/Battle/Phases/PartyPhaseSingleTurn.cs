using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PartyPhaseSingleTurn : PartyPhase
{
    public override Cursor Cursor => cursor;
    [SerializeField]
    private Cursor cursor;
    public Queue<PartyMember> PartyTurnOrder { get; private set; } = null;
    private PartyMember activeTurnMember;

    private void Initialize()
    {
        PartyTurnOrder = new Queue<PartyMember>();
        Party.Sort((p, p2) => p2.Col.CompareTo(p.Col));
        foreach (var member in Party)
            PartyTurnOrder.Enqueue(member);
    }

    public override Coroutine OnPhaseEnd()
    {
        Cursor.gameObject.SetActive(false);
        PartyTurnOrder.Enqueue(activeTurnMember);
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        if (PartyTurnOrder == null)
            Initialize();
        activeTurnMember = null;
        while(activeTurnMember == null && PartyTurnOrder.Count > 0)
        {
            activeTurnMember = PartyTurnOrder.Dequeue();
        }
        StartCoroutine(TurnCr());
        return null;
    }

    private IEnumerator TurnCr()
    {
        yield return activeTurnMember.StartTurn();
        Cursor.Highlight(activeTurnMember.Pos);
        Cursor.SetActive(true);
    }

    public override void OnPhaseUpdate() { }

    public override void EndAction(PartyMember member)
    {
        EndPhase();
    }
}
