using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartyPhase : Phase
{
    public abstract Cursor Cursor { get; }
    public List<PartyMember> Party { get; } = new List<PartyMember>();

    public abstract void EndAction(PartyMember member);

    public virtual void CancelAction(PartyMember p)
    {
        Cursor.SetActive(true);
    }
}
