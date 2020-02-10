using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class BattleUI : MonoBehaviour
{
    public static BattleUI main;

    [Header("Info Panel Fields")]
    public GameObject infoPanel;
    public GameObject infoPanelCombatant;
    public GameObject infoPanelParty;
    public GameObject infoPanelEnemy;
    public GameObject infoPanelObstacle;
    public Image infoPanelImage;
    public Image infoPanelBg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI descriptionText;

    [Header("Generic Combatant Info")]
    public Image genericHpBarImage;
    public TextMeshProUGUI genericHpText;

    [Header("Party-Specific Info")]
    public Image partyHpBarImage;
    public TextMeshProUGUI partyHpText;
    public Image partyFpBarImage;
    public TextMeshProUGUI partyFpText;

    [Header("Enemy-Specific Info")]
    public Image enemyHpBarImage;
    public TextMeshProUGUI enemyHpText;
    public AttackDescriptionUI enemyAttackUI;

    [Header("Colors")]
    public Color partyColor;
    public Color neutralColor;
    public Color enemyColor;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        infoPanelCombatant.SetActive(false);
        infoPanelEnemy.SetActive(false);
        infoPanelParty.SetActive(false);
        infoPanelObstacle.SetActive(false);
    }

    public void ShowInfoPanelEnemy(Enemy e)
    {
        InitializeInfoPanel(e);
        infoPanelEnemy.SetActive(true);
        enemyHpText.text = e.Hp.ToString();
        enemyHpBarImage.fillAmount = e.Hp / (float)e.maxHp;
        enemyAttackUI.ShowAttack(e.action);
    }

    public void ShowInfoPanelParty(PartyMember p)
    {
        InitializeInfoPanel(p);
        infoPanelParty.SetActive(true);
        partyHpText.text = p.Hp.ToString();
        partyHpBarImage.fillAmount = p.Hp / (float)p.maxHp;
        partyFpText.text = p.Fp.ToString();
        partyFpBarImage.fillAmount = p.Fp / (float)p.maxFp;
    }

    public void ShowInfoPanelGeneric(Combatant c)
    {
        InitializeInfoPanel(c);
        infoPanelObstacle.SetActive(true);
        genericHpText.text = c.Hp.ToString();
        genericHpBarImage.fillAmount = c.Hp / (float)c.maxHp; 
    }

    private void InitializeInfoPanel(Combatant c)
    {
        infoPanel.SetActive(true);
        infoPanelCombatant.SetActive(true);
        nameText.text = c.DisplayName;
        moveNumberText.text = c.Move.ToString();
        descriptionText.text = c.description;
        // Set Image panel sprite and color from combatant properties
        infoPanelImage.sprite = c.DisplaySprite;
        infoPanelImage.color = c.DisplaySpriteColor;
        // Set Image Panel Bg color according toallegiance
        var bgColor = neutralColor;
        if (c.Team == FieldEntity.Teams.Party)
            bgColor = partyColor;
        else if (c.Team == FieldEntity.Teams.Enemy)
            bgColor = enemyColor;
        infoPanelBg.color = bgColor;
    }
}
