using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPhase : Phase
{
    public List<Enemy> Enemies { get; } = new List<Enemy>();
}
