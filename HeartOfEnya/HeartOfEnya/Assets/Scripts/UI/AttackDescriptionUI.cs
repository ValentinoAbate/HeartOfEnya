using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AttackDescriptionUI : MonoBehaviour
{
    private const string directionalRangeText = "1";
    private const int maxPatternCols = 3;
    private const int maxPatternRows = 3;
    private const int patternMiddleRow = maxPatternRows / 2 + maxPatternRows % 2;
    private const int patternMiddleCol = maxPatternCols / 2 + maxPatternCols % 2;

    [Header("UI References")]
    public TextMeshProUGUI damageNumberText;
    public TextMeshProUGUI rangeTypeText;
    public TextMeshProUGUI[] effectTexts;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI burnDescriptionText;
    [Header("Attack Range Display")]
    public GameObject gridLayout;
    public Image userIconImage;
    public GameObject squarePrefab;
    public Sprite attackIconEmpty;
    public Sprite attackIconHit;
    public Sprite attackIconUser;
    public Sprite attackIconCenter;
    private List<GameObject> attackIcons = new List<GameObject>();

    public void ShowAttack(Action action)
    {
        var pattern = action.targetPattern;
        var effects = action.GetComponents<ActionEffect>();
        bool burns = action.GetComponent<StunEffect>() != null;
        var userPos = Pos.Zero;
        var targetPos = new Pos(patternMiddleRow - 1, patternMiddleCol - 1);
        // Main panel code
        nameText.text = action.DisplayName;
        descriptionText.text = action.Description;
        burnDescriptionText.gameObject.SetActive(burns);
        // Detail panel code
        int damage = effects.Sum((effect) => effect is DamageEffect ? (effect as DamageEffect).damage : 0);
        damageNumberText.text = damage.ToString();
        if (pattern.type == TargetPattern.Type.Spread)
        {
            rangeTypeText.text = action.range.ToString();
            userIconImage.gameObject.SetActive(false);
        }
        else
        {
            userPos = new Pos(patternMiddleRow - 1, maxPatternCols);
            targetPos = userPos + Pos.Left;
            rangeTypeText.text = directionalRangeText;
            userIconImage.gameObject.SetActive(true);
        }
        int i = 0;
        while (i < action.detailTexts.Length && i < effectTexts.Length)
        {
            effectTexts[i].text = action.detailTexts[i];
            effectTexts[i].fontStyle = FontStyles.Normal;
            ++i;
        }
        if (i < effectTexts.Length && burns)
        {
            effectTexts[i].text = "Burns";
            effectTexts[i].fontStyle = FontStyles.Italic;
            ++i;
        }
        if (i < effectTexts.Length && action.chargeTurns > 0)
        {
            effectTexts[i].text = "Charged";
            effectTexts[i].fontStyle = FontStyles.Italic;
            ++i;
        }
        while (i < effectTexts.Length)
        {
            effectTexts[i].text = string.Empty;
            effectTexts[i].fontStyle = FontStyles.Normal;
            ++i;
        }
        // Target pattern panel code
        foreach (var icon in attackIcons)
            Destroy(icon);
        attackIcons.Clear();
        var positions = action.HitPositions(userPos, targetPos);
        for (int row = 0; row < maxPatternRows; ++row)
        {
            for (int col = 0; col < maxPatternCols; ++col)
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
