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
    public SpriteRenderer sprite;
    public override Sprite DisplaySprite => sprite.sprite;
    public override Color DisplaySpriteColor => sprite.color;
    public override Teams Team => Teams.Neutral;
    public override void Highlight()
    {
        BattleUI.main.ShowInfoPanelObstacle(this);
    }

    public override void UnHighlight()
    {
        BattleUI.main.HideInfoPanel();
    }
}
