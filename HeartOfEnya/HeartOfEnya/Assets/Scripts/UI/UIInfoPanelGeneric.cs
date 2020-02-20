using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfoPanelGeneric : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image unitImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI hpNumberText;
    public Image hpBarImage;
    public void ShowUI(Combatant c)
    {
        nameText.text = c.DisplayName;
        descriptionText.text = c.description;
        statusText.text = "status";
        moveNumberText.text = c.Move.ToString();
        hpNumberText.text = c.Hp.ToString();
        hpBarImage.fillAmount = c.Hp / (float)c.maxHp;
        unitImage.sprite = c.DisplaySprite;
        unitImage.color = c.DisplaySpriteColor;
    }
}
