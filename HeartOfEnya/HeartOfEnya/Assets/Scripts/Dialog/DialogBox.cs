using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental;
using System;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour, IPausable
{
    public enum State
    {
        Inactive,
        Scrolling,
        Cancel,
        Waiting,
    }
    public PauseHandle PauseHandle { get; set; } = new PauseHandle();
    public TextMeshProUGUI text;
    public Image portrait;
    public string VoiceEvent { get => voiceEmitter.Event; set => voiceEmitter = GameObject.Find(value).GetComponent<FMODUnity.StudioEventEmitter>(); }

    private State state = State.Inactive;
    private bool finishedWithStartAnimation = true;
    private FMODUnity.StudioEventEmitter voiceEmitter;

    private void OnConfirm()
    {
        if (PauseHandle.Paused)
            return;
        if(state != State.Inactive)
            state = state.Next();
    }

    public IEnumerator PlayLine(string line, float scrollDelay, Sprite portrait = null, string voiceEvent = null)
    {
        // Set Portrait if desired
        if (portrait != null)
            this.portrait.sprite = portrait;
        if (voiceEvent != null)
            VoiceEvent = voiceEvent;
        yield return new WaitWhile(() => PauseHandle.Paused);
        // If we are starting up, wait until finished starting
        yield return new WaitUntil(() => finishedWithStartAnimation);
        // Scroll if there is a scroll delay
        if (scrollDelay > 0)
        {
            // Start the text scroll effect
            voiceEmitter.Play();
            state = State.Scrolling;
            text.text = string.Empty;
            for (int i = 0; state != State.Cancel && i < line.Length - 1; ++i)
            {
                yield return new WaitWhile(() => PauseHandle.Paused);
                voiceEmitter.SetParameter("Space", char.IsWhiteSpace(line[i]) ? 1 : 0);
                text.text += line[i];
                yield return new WaitForSeconds(scrollDelay);
            }
        }
        // Dump text
        text.text = line;
        // Wait until the player continues
        state = State.Waiting;
        voiceEmitter.Stop();
        yield return new WaitWhile(() => state == State.Waiting);
        state = State.Inactive;
    }
}
