using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AttackDescriptionUI : MonoBehaviour
{
    private const int maxPatternCols = 9;
    private const int maxPatternRows = 4;
    private const int patternMiddleRow = maxPatternRows / 2 + maxPatternRows % 2;
    private const int patternMiddleCol = maxPatternCols / 2 + maxPatternCols % 2;

    [Header("UI References")]
    public TextMeshProUGUI damageNumberText;
    public TextMeshProUGUI rangeTypeText;
    public TextMeshProUGUI chargeText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    [Header("Attack Range Display")]
    public GameObject gridLayout;
    public GameObject squarePrefab;
    public Sprite attackIconEmpty;
    public Sprite attackIconHit;
    public Sprite attackIconUser;
    public Sprite attackIconCenter;
    private List<GameObject> attackIcons = new List<GameObject>();

    public void ShowAttack(Action action)
    {
        transform.position = new Vector3(-1, -4.5f);
        var pattern = action.targetPattern;
        var effects = action.GetComponents<ActionEffect>();
        int damage = effects.Sum((effect) => effect is DamageEffect ? (effect as DamageEffect).damage : 0);
        damageNumberText.text = damage.ToString();
        chargeText.text = action.chargeTurns == 0 ? "Immediate" : "Charge Turns: " + action.chargeTurns.ToString();
        nameText.text = action.DisplayName;
        descriptionText.text = action.Description;
        foreach (var icon in attackIcons)
            Destroy(icon);
        attackIcons.Clear();
        var userPos = Pos.Zero;
        var targetPos = new Pos(patternMiddleRow, patternMiddleCol);
        if (pattern.type == TargetPattern.Type.Spread)
        {
            rangeTypeText.text = action.range.ToString();
        }
        else
        {
            userPos = new Pos(patternMiddleRow, patternMiddleCol + pattern.maxReach.x / 2);
            targetPos = userPos + Pos.Left;
            rangeTypeText.text = "Directional";
        }
        var positions = action.HitPositions(userPos, targetPos);
        for (int row = 0; row <= maxPatternRows; ++row)
        {
            for (int col = 0; col <= maxPatternCols; ++col)
            {
                var pos = new Pos(row, col);
                var icon = Instantiate(squarePrefab, gridLayout.transform);
                if(pattern.type == TargetPattern.Type.Directional && pos == userPos)
                {
                    var image = icon.GetComponent<Image>();
                    image.sprite = attackIconUser;
                }
                if (positions.Contains(new Pos(row, col)))
                {
                    var image = icon.GetComponent<Image>();
                    image.sprite = attackIconHit;
                }
                attackIcons.Add(icon);
            }
        }


    }
}
