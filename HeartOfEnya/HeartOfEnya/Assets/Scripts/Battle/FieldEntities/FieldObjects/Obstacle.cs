using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A combatant considered to be an obstacle.
/// Exactly like a generic combatant but is always neutral
/// </summary>
public class Obstacle : Combatant
{
    public override Teams Team => Teams.Neutral;
}
