using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Willow : Combatant
{
    public override Teams Team => Teams.Party;
    public override Sprite DisplaySprite => throw new System.NotImplementedException();

    public override Color DisplaySpriteColor => throw new System.NotImplementedException();

    protected override void Initialize()
    {
        Pos = Pos.OutOfBounds;
    }
}
