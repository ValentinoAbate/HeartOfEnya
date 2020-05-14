using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[DisallowMultipleComponent]
public class BattleUI : MonoBehaviour, IPausable
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
    public PauseHandle PauseHandle { get; set; }

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
        if (pData.InTutorialFirstDay)
            DisableRunTiles();
        PauseHandle = new PauseHandle(OnPause);
        PhaseManager.main.PartyPhase.PauseHandle.Dependents.Add(this);
    }
    private bool savedEndTurnButtonVal = false;
    private Enemy savedEnemyInspect = null;
    private PartyMember savedPartyInspect = null;
    private Combatant savedGenericInspect = null;
    private void OnPause(bool pause)
    {
        if(pause)
        {
            savedEndTurnButtonVal = endTurnButton.interactable;
            HideEndTurnButton();
            HideInfoPanel(false);
        }
        else
        {
            if (savedEndTurnButtonVal)
                ShowEndTurnButton();
            if (savedEnemyInspect != null)
                ShowInfoPanelEnemy(savedEnemyInspect);
            else if (savedPartyInspect != null)
                ShowInfoPanelParty(savedPartyInspect);
            else if (savedGenericInspect != null)
                ShowInfoPanelGeneric(savedGenericInspect);
        }
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

    public void EnableEndTurnButton() => endTurnButton.gameObject.SetActive(true);

    public void DisableEndTurnButton() => endTurnButton.gameObject.SetActive(false);

    public void HideInfoPanel(bool clear = true)
    {
        infoPanel.SetActive(false);
        enemyInfoPanel.gameObject.SetActive(false);
        genericInfoPanel.gameObject.SetActive(false);
        partyInfoPanel.gameObject.SetActive(false);
        if(clear)
        {
            savedEnemyInspect = null;
            savedGenericInspect = null;
            savedPartyInspect = null;
        }
    }

    public void ShowInfoPanelEnemy(Enemy e)
    {
        InitializeInfoPanel();
        enemyInfoPanel.gameObject.SetActive(true);
        enemyInfoPanel.ShowUI(e);
        savedEnemyInspect = e;
    }

    public void ShowInfoPanelParty(PartyMember p)
    {
        InitializeInfoPanel();
        partyInfoPanel.gameObject.SetActive(true);
        partyInfoPanel.ShowUI(p);
        savedPartyInspect = p;
    }

    public void ShowInfoPanelGeneric(Combatant c)
    {
        InitializeInfoPanel();
        genericInfoPanel.gameObject.SetActive(true);
        genericInfoPanel.ShowUI(c);
        savedGenericInspect = c;
    }

    private void InitializeInfoPanel()
    {
        infoPanel.SetActive(true);
    }
}
