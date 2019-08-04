using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental;
using System;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour
{
    public enum State
    {
        Inactive,
        Scrolling,
        Cancel,
        Waiting,
    }
    public TextMeshProUGUI text;
    public Image portrait;

    private State state = State.Inactive;
    private bool finishedWithStartAnimation = true;


    private void OnConfirm()
    {
        if(state != State.Inactive)
            state = state.Next();
    }

    public IEnumerator PlayLine(string line, float scrollDelay, Sprite portrait = null)
    {
        // Set Portrait if desired
        if (portrait != null)
            this.portrait.sprite = portrait;
        // If we are starting up, wait until finished starting
        yield return new WaitUntil(() => finishedWithStartAnimation);
        // Scroll if there is a scroll delay
        if (scrollDelay > 0)
        {
            state = State.Scrolling;
            text.text = string.Empty;
            for (int i = 0; state != State.Cancel && i < line.Length - 1; ++i)
            {
                text.text += line[i];
                yield return new WaitForSeconds(scrollDelay);
            }
        }
        // Dump text
        text.text = line;
        // Wait until the player continues
        state = State.Waiting;
        yield return new WaitWhile(() => state == State.Waiting);
        state = State.Inactive;
    }
}
