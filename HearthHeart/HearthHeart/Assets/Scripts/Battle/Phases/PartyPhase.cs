using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartyPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; }
    public abstract Cursor Cursor { get; }
    public List<PartyMember> Party { get; } = new List<PartyMember>();

    private void Awake()
    {
        PauseHandle = new PauseHandle(OnPause);
    }
    public virtual void OnPause(bool pause)
    {
        if(pause)
        {
            Cursor.PauseHandle.Pause(PauseHandle.PauseSources);
        }
        else
        {
            Cursor.PauseHandle.UnPauseAll();
        }       
    }
    public abstract void EndAction(PartyMember member);
    public virtual void CancelAction(PartyMember p)
    {
        Cursor.SetActive(true);
    }
}
