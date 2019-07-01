using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Combatant
{
    protected override void Initialize()
    {
        base.Initialize();
        PhaseManager.main?.EnemyPhase.Enemies.Add(this);
    }
}
