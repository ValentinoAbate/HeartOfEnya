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
    }

    private void InitializeInfoPanel(Combatant c)
    {
        infoPanel.SetActive(true);
        infoPanelCombatant.SetActive(true);
        nameText.text = c.DisplayName;
        moveNumberText.text = c.Move.ToString();
        descriptionText.text = c.description;     
    }
}
