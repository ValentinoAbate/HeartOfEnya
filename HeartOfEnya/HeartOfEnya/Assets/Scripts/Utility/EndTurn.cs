using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public void End()
    {
        if (PhaseManager.main.Transitioning)
            return;
        if (PhaseManager.main.ActivePhase == PhaseManager.main.PartyPhase)
        {
            BattleUI.main.HideInfoPanel();
            PhaseManager.main.NextPhase();
        }
    }
}
