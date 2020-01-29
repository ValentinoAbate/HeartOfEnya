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
    public GameObject infoPanelImmuneObj;
    public GameObject infoPanelVulnerableObj;
    public Image infoPanelImage;
    public Image infoPanelBg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI descriptionText;

    [Header("Party-Specific Info")]
    public Image partyHpBarImage;
    public TextMeshProUGUI partyHpText;
    public Image partyFpBarImage;
    public TextMeshProUGUI partyFpText;

    [Header("Enemy-Specific Info")]
    public Image enemyHpBarImage;
    public TextMeshProUGUI enemyHpText;

    [Header("Obstacle-Specific Info")]
    public Image obstacleHpBarImage;
    public TextMeshProUGUI obstacleHpText;

    [Header("Colors")]
    public Color partyColor;
    public Color neutralColor;
    public Color enemyColor;

    [Header("IconImages")]
    public Sprite iconFire;
    public Sprite iconIce;
    public Sprite iconPhys;
    public Sprite iconMag;
    public Sprite iconSupport;
    public Sprite iconNone;
    public Dictionary<ActionEffect.Attribute, Sprite> iconSprites;

    private Image[] immuneImages;
    private Image[] vulnerableImages;

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
        immuneImages = infoPanelImmuneObj.GetComponentsInChildren<Image>();
        vulnerableImages = infoPanelVulnerableObj.GetComponentsInChildren<Image>();
        iconSprites = new Dictionary<ActionEffect.Attribute, Sprite>
        {
            {ActionEffect.Attribute.Physical, iconPhys },
            {ActionEffect.Attribute.Magic,    iconMag },
            {ActionEffect.Attribute.Fire,     iconFire },
            {ActionEffect.Attribute.Ice,      iconIce },
            {ActionEffect.Attribute.Support,  iconSupport },
        };
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
    }

    public void ShowInfoPanelParty(PartyMember e)
    {
        InitializeInfoPanel(e);
        infoPanelParty.SetActive(true);
        partyHpText.text = e.Hp.ToString();
        partyFpText.text = e.Fp.ToString();
    }

    public void ShowInfoPanelObstacle(Obstacle e)
    {
        InitializeInfoPanel(e);
        infoPanelObstacle.SetActive(true);
        obstacleHpText.text = e.Hp.ToString();
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
        int immuneInd = 0;
        int vulnerableInd = 0;
        // Set Element Icons
        foreach(var element in iconSprites.Keys)
        {
            var reaction = c.reactions[element];
            if(reaction == ActionEffect.Reaction.Immune)
                immuneImages[immuneInd++].sprite = iconSprites[element];
            else if(reaction == ActionEffect.Reaction.Vulnerable)
                vulnerableImages[vulnerableInd++].sprite = iconSprites[element];
        }
        for (int i = immuneInd; i < immuneImages.Length; ++i)
            immuneImages[i].sprite = iconNone;
        for (int i = vulnerableInd; i < vulnerableImages.Length; ++i)
            vulnerableImages[i].sprite = iconNone;
    }
}
