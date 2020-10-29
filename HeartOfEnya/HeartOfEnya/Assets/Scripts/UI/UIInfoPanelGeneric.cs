using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfoPanelGeneric : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI unitPropertiesText;
    public TextMeshProUGUI[] effectTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI hpNumberText;
    public TextMeshProUGUI damageNumberText;
    public Image hpBarImage;

    public void ShowUI(Combatant c)
    {
        nameText.text = c.DisplayName;
        descriptionText.text = c.description;
        //moveNumberText.text = e.Move.ToString();
        hpNumberText.text = c.Hp.ToString();
        hpBarImage.fillAmount = c.Hp / (float)c.maxHp;
        descriptionText.text = c.description;
        unitPropertiesText.text = c.isMovable ? string.Empty : "Immovable";
        damageNumberText.text = "N/A";
        effectTexts[0].text = "No Actions";

    }
}
