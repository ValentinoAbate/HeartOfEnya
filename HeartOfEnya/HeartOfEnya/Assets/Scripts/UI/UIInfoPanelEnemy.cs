using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfoPanelEnemy : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image unitImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI hpNumberText;
    public Image hpBarImage;
    public AttackDescriptionUI attackUI;

    public void ShowUI(Enemy e)
    {
        nameText.text = e.DisplayName;
        descriptionText.text = e.description;
        statusText.text = "status";
        moveNumberText.text = e.Move.ToString();
        hpNumberText.text = e.Hp.ToString();
        attackUI.ShowAttack(e.action);
        unitImage.sprite = e.DisplaySprite;
        unitImage.color = e.DisplaySpriteColor;
    }

}
