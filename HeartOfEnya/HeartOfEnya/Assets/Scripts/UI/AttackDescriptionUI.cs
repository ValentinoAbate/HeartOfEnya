using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AttackDescriptionUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI damageNumberText;
    public TextMeshProUGUI rangeTypeText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    [Header("Attack Range Display")]
    public GameObject gridLayout;
    public GameObject squarePrefab;
    public Sprite attackIconEmpty;
    public Sprite attackIconHit;
    public Sprite attackIconUser;
    public Sprite attackIconCenter;
    private static readonly Pos userPosDirectional = new Pos(2, 3);
    private List<GameObject> attackIcons = new List<GameObject>();

    public void ShowAttack(Action action)
    {
        var pattern = action.targetPattern;
        var effects = action.GetComponents<ActionEffect>();
        int damage = effects.Sum((effect) => effect is DamageEffect ? (effect as DamageEffect).damage : 0);
        damageNumberText.text = damage.ToString();
        nameText.text = action.DisplayName;
        descriptionText.text = action.Description;
        foreach (var icon in attackIcons)
            Destroy(icon);
        attackIcons.Clear();
        if (pattern.type == TargetPattern.Type.Spread)
        {
            pattern.Target(Pos.Zero, new Pos(2, 4));
            rangeTypeText.text = action.range.ToString();
        }
        else
        {
            pattern.Target(userPosDirectional, userPosDirectional + Pos.Right);
            rangeTypeText.text = "Directional";
        }
        for (int row = 0; row <= 4; ++row)
        {
            for (int col = 0; col <= 9; ++col)
            {
                var pos = new Pos(row, col);
                var icon = Instantiate(squarePrefab, gridLayout.transform);
                if(pattern.type == TargetPattern.Type.Directional && pos == userPosDirectional + Pos.Right)
                {
                    var image = icon.GetComponent<Image>();
                    image.sprite = attackIconUser;
                }
                if (pattern.Positions.Contains(new Pos(row, col)))
                {
                    var image = icon.GetComponent<Image>();
                    image.sprite = attackIconHit;
                }
                attackIcons.Add(icon);
            }
        }


    }
}
