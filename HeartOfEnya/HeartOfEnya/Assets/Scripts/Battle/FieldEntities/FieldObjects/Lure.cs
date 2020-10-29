using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Lure : Combatant
{
    public SpriteRenderer sprite;
    public override Sprite DisplaySprite => sprite.sprite;
    public override Color DisplaySpriteColor => sprite.color;
    public override Teams Team => Teams.Neutral;
}
