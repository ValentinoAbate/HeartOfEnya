using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Combatant
{
    public override Team Allegiance { get => Team.Enemy; }
    protected override void Initialize()
    {
        base.Initialize();
        PhaseManager.main?.EnemyPhase.Enemies.Add(this);
    }
}
