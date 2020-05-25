using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Button))]
public class ActionButton : ActionButtonBase
{
    public override string ID => actionPrefab == null ? "none" : actionPrefab.ID;

    [Header("Set to configure attack")]
    public Action actionPrefab;
    [Header("Set in ActionMenu prefab")]
    public ActionMenu menu;
    [Header("Set in ActionButton prefab")]
    public Button button;
    public EventTrigger trigger;
    public TextMeshProUGUI text;
    public GameObject extraInfoWindowPrefab;

    private FMODUnity.StudioEventEmitter sfxSelect;
    private FMODUnity.StudioEventEmitter sfxHighlight;

    private void Awake()
    {
        sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxHighlight = GameObject.Find("UIHighlight").GetComponent<FMODUnity.StudioEventEmitter>();
        
        // Set up click listeners
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(sfxSelect.Play);        
        button.onClick.AddListener(menu.cursor.CalculateTargets);
        button.onClick.AddListener(menu.Close);
        button.onClick.AddListener(() => menu.cursor.SetActive(true));

        // Set up select event triggers
        trigger.triggers.Clear();
        AddEntry(EventTriggerType.PointerEnter, OnSelect);
        AddEntry(EventTriggerType.PointerExit, OnDeselect);
        //AddEntry(EventTriggerType.Select, OnSelect);
        //AddEntry(EventTriggerType.Deselect, OnDeselect);

        // Set button text
        text.text = actionPrefab.DisplayName;
    }

    private void AddEntry(EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable)
            return;
        sfxHighlight.Play();
        menu.cursor.SetAction(actionPrefab);
        menu.cursor.ShowTargets();
        ShowExtraInfoWindow();
        eventData.Use();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!button.interactable)
            return;
        menu.cursor.HideTargets();
        HideExtraInfoWindow();
        eventData.Use();
    }

    public void HideExtraInfoWindow()
    {
        BattleUI.main.HideAttackDescriptionPanel();
        BattleUI.main.RestoreInfoPanel();
    }

    public void ShowExtraInfoWindow()
    {
        BattleUI.main.ShowAttackDescriptionPanel(extraInfoWindowPrefab, actionPrefab);
        BattleUI.main.HideInfoPanel(false);
    }
}
