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

    [SerializeField]
    private GameObject enemiesRemainingUI;
    public TextMeshProUGUI numEnemiesLeft;

    public Button endTurnButton;

    [Header("Info Panel Fields")]
    public GameObject infoPanel;
    public UIInfoPanelEnemy enemyInfoPanel;
    public UIInfoPanelGeneric genericInfoPanel;
    public UIInfoPanelParty partyInfoPanel;
    public GameObject attackInfoContainer;
    private AttackDescriptionUI attackInfoPanel;

    [Header("Colors")]
    public Color partyColor;
    public Color neutralColor;
    public Color enemyColor;

    [Header("Prompts")]
    [SerializeField] private GameObject prompt = null;
    [SerializeField] private TextMeshProUGUI promptText = null;

    public bool CancelingEnabled { get; set; } = true;
    public HashSet<Pos> MoveableTiles { get; set; } = new HashSet<Pos>();
    public HashSet<Pos> TargetableTiles { get; set; } = new HashSet<Pos>();
    public PauseHandle PauseHandle { get; set; }

    [Header("Set in Scene")]
    public GameObject runTileContainer;
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
        runTiles = runTileContainer.GetComponentsInChildren<EventTileAction>().ToList();
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.InTutorialFirstDay || pData.InTutorialSecondDay || pData.InTutorialThirdDay)
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
            RestoreInfoPanel();
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
        if (pData.InMainPhase || pData.absoluteZeroPhase1Defeated)
        {
            if(pData.numEnemiesLeft == 1)
            {
                numEnemiesLeft.text = numRemaining.ToString() + " Frost Remains!";
            }
            else if(pData.numEnemiesLeft <= 0)
            {
                numEnemiesLeft.text = "Absolute Zero Remains";
            }
            else
                numEnemiesLeft.text = numRemaining.ToString() + " Frost Remain...";
        }
        else
        {
            numEnemiesLeft.text =  "??? Frost Remain...";
        }

    }

    public void ShowEnemiesRemaining()
    {
        enemiesRemainingUI.SetActive(true);
    }

    public void HideEnemiesRemaining()
    {
        enemiesRemainingUI.SetActive(false);
    }

    private bool hideEndTurnButtonHighPriority = false;

    public void ShowEndTurnButton(bool highPriority = false)
    {
        if (highPriority)
            hideEndTurnButtonHighPriority = false;
        if (hideEndTurnButtonHighPriority)
            return;
        endTurnButton.interactable = true;
    }

    public void HideEndTurnButton(bool highPriority = false)
    {
        endTurnButton.interactable = false;
        if (highPriority)
            hideEndTurnButtonHighPriority = true;
    }

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

    public void RestoreInfoPanel()
    {
        if (savedEnemyInspect != null)
            ShowInfoPanelEnemy(savedEnemyInspect);
        else if (savedPartyInspect != null)
            ShowInfoPanelParty(savedPartyInspect);
        else if (savedGenericInspect != null)
            ShowInfoPanelGeneric(savedGenericInspect);
    }

    public void ShowAttackDescriptionPanel(GameObject panelPrefab, Action action)
    {
        var panel = Instantiate(panelPrefab, attackInfoContainer.transform);
        //panel.transform.localPosition = Vector3.zero;
        var ui = panel.GetComponent<AttackDescriptionUI>();
        if(ui != null)
        {
            HideAttackDescriptionPanel();
            attackInfoPanel = ui;
            ui.ShowAttack(action);
        }
    }

    public void HideAttackDescriptionPanel()
    {
        if (attackInfoPanel == null)
            return;
        Destroy(attackInfoPanel.gameObject);
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

    public void ShowPrompt(string text)
    {
        prompt.SetActive(true);
        promptText.text = text;
    }

    public void HidePrompt()
    {
        prompt.SetActive(false);
    }
}
