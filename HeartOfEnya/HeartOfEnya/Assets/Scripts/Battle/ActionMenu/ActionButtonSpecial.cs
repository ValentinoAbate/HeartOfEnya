using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// script for playing sound effects on special action buttons
public class ActionButtonSpecial : ActionButtonBase
{
    public override string ID => id;
    public Button button;
    public EventTrigger trigger;
    [SerializeField]
    private string id;

    private FMODUnity.StudioEventEmitter sfxSelect;
    private FMODUnity.StudioEventEmitter sfxHighlight;

    private void Awake()
    {
    	sfxSelect = GameObject.Find("UISelect").GetComponent<FMODUnity.StudioEventEmitter>();
        sfxHighlight = GameObject.Find("UIHighlight").GetComponent<FMODUnity.StudioEventEmitter>();

        button.onClick.AddListener(sfxSelect.Play);
        AddEntry(EventTriggerType.PointerEnter, OnSelect);
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
    	sfxHighlight.Play();
    	eventData.Use();
    }
}
