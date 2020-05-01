using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[DisallowMultipleComponent]
public class BattleUI : MonoBehaviour
{
    public static BattleUI main;

    public TextMeshProUGUI numEnemiesLeft;

    public Button endTurnButton;

    [Header("Info Panel Fields")]
    public GameObject infoPanel;
    public UIInfoPanelEnemy enemyInfoPanel;
    public UIInfoPanelGeneric genericInfoPanel;
    public UIInfoPanelParty partyInfoPanel;

    [Header("Colors")]
    public Color partyColor;
    public Color neutralColor;
    public Color enemyColor;

    public bool CancelingEnabled { get; set; } = true;
    public HashSet<Pos> MoveableTiles { get; set; } = new HashSet<Pos>();
    public HashSet<Pos> TargetableTiles { get; set; } = new HashSet<Pos>();

    private List<EventTileAction> runTiles;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        runTiles = GetComponentsInChildren<EventTileAction>().ToList();
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.gamePhase == PersistentData.gamePhaseTutorial)
            DisableRunTiles();
    }

    public void DisableRunTiles()
    {
        foreach(var tile in runTiles)
        {
            BattleGrid.main.RemoveEventTile(tile.Pos, tile);
            tile.gameObject.SetActive(false);
        }
    }

    public void EnableRunTiles()
    {
        foreach (var tile in runTiles)
        {
            BattleGrid.main.AddEventTile(tile.Pos, tile);
            tile.gameObject.SetActive(true);
        }
    }

    public void UpdateEnemiesRemaining(int numRemaining)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.InMainPhase || pData.absoluteZeroDefeated)
        {
            if(pData.numEnemiesLeft == 1)
            {
                numEnemiesLeft.text = numRemaining.ToString() + " Frost Remains!";
            }
            else if(pData.numEnemiesLeft <= 0)
            {
                numEnemiesLeft.text = "No Frost Remain";
            }
            else
                numEnemiesLeft.text = numRemaining.ToString() + " Frost Remain...";
        }
        else
        {
            numEnemiesLeft.text =  "??? Frost Remain...";
        }

    }

    public void ShowEndTurnButton() => endTurnButton.interactable = true;

    public void HideEndTurnButton() => endTurnButton.interactable = false;

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        enemyInfoPanel.gameObject.SetActive(false);
        genericInfoPanel.gameObject.SetActive(false);
        partyInfoPanel.gameObject.SetActive(false);
    }

    public void ShowInfoPanelEnemy(Enemy e)
    {
        InitializeInfoPanel(e);
        enemyInfoPanel.gameObject.SetActive(true);
        enemyInfoPanel.ShowUI(e);
    }

    public void ShowInfoPanelParty(PartyMember p)
    {
        InitializeInfoPanel(p);
        partyInfoPanel.gameObject.SetActive(true);
        partyInfoPanel.ShowUI(p);
    }

    public void ShowInfoPanelGeneric(Combatant c)
    {
        InitializeInfoPanel(c);
        genericInfoPanel.gameObject.SetActive(true);
        genericInfoPanel.ShowUI(c);
    }

    private void InitializeInfoPanel(Combatant c)
    {
        infoPanel.SetActive(true);
    }
}
