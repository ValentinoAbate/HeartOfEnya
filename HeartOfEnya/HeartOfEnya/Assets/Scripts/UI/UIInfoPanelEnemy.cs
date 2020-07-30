using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfoPanelEnemy : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI unitPropertiesText;
    public TextMeshProUGUI[] effectTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI hpNumberText;
    public TextMeshProUGUI damageNumberText;
    public Image hpBarImage;

    public void ShowUI(Enemy e)
    {
        nameText.text = e.DisplayName;
        descriptionText.text = e.description;
        moveNumberText.text = e.Move.ToString();
        hpNumberText.text = e.Hp.ToString();
        hpBarImage.fillAmount = e.Hp / (float)e.maxHp;
        descriptionText.text = e.description;
        unitPropertiesText.text = e.isBoss ? "Immovable, Unburnable, Acts Last, Defeat to End Battle" : string.Empty;
        int damage = e.action.TotalDamage;
        damageNumberText.text = damage > 0 ? damage.ToString() : "N/A";
        int i = 0;
        while(i < e.action.detailTexts.Length && i < effectTexts.Length)
        {
            effectTexts[i].text = e.action.detailTexts[i];
            effectTexts[i].fontStyle = FontStyles.Normal;
            ++i;
        }
        if(i < effectTexts.Length && e.action.chargeTurns > 0)
        {
            effectTexts[i].text = "Charged";
            effectTexts[i].fontStyle = FontStyles.Italic;
            ++i;
        }
        while(i < effectTexts.Length)
        {
            effectTexts[i].text = string.Empty;
            effectTexts[i].fontStyle = FontStyles.Normal;
            ++i;
        }
        
    }

}
