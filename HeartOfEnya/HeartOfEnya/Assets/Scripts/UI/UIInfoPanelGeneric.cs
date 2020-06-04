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

    public void ShowUI(Combatant e)
    {
        nameText.text = e.DisplayName;
        descriptionText.text = e.description;
        //moveNumberText.text = e.Move.ToString();
        hpNumberText.text = e.Hp.ToString();
        descriptionText.text = e.description;
        unitPropertiesText.text = e.isMovable ? string.Empty : "Immovable";
        damageNumberText.text = "N/A";
        effectTexts[0].text = "No Actions";

    }
}
