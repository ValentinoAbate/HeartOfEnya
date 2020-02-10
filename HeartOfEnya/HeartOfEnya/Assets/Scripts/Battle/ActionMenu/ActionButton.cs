using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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

        // Set up select event triggers
        trigger.triggers.Clear();
        AddEntry(EventTriggerType.PointerEnter, OnSelect);
        AddEntry(EventTriggerType.PointerExit, OnDeselect);
        AddEntry(EventTriggerType.Select, OnSelect);
        AddEntry(EventTriggerType.Deselect, OnDeselect);

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
        menu.cursor.SetAction(actionPrefab);
        menu.cursor.ShowTargets();
        eventData.Use();
    }
    public void OnDeselect(BaseEventData eventData)
    {       
        menu.cursor.HideTargets();
        eventData.Use();
    }

    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("OnPointerExit called.");
        OnDeselect(data);
    }
}
