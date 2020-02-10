using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActionButton : MonoBehaviour
{
    [Header("Set to configure attack")]
    public Action actionPrefab;
    [Header("Set in ActionMenu prefab")]
    public ActionMenu menu;
    [Header("Set in ActionButton prefab")]
    public Button button;
    public EventTrigger trigger;
    public TextMeshProUGUI text;

    private void Awake()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(menu.Close);
        button.onClick.AddListener(menu.cursor.CalculateTargets);
        button.onClick.AddListener(() => menu.cursor.SetActive(true));
        // Set up select event trigger
        EventTrigger.Entry selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Select;
        selectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(selectEntry);
        // Set up deselect event trigger
        EventTrigger.Entry deselectEntryEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.Deselect;
        selectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(selectEntry);
        text.text = actionPrefab.DisplayName;
    }

    public void OnSelect(BaseEventData eventData)
    {
        menu.cursor.SetAction(actionPrefab);
        menu.cursor.ShowTargets();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        menu.cursor.HideTargets();
    }
}
