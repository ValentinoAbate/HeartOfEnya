using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

// The basic idea here is that the FMOD event starts when the text starts scrolling and has its "Space" parameter
// set to true periodically based on the text to create pauses.
// It's important to note that the event is not being triggered again for each character written, but rather runs parallel to it
public class TestTextBlips : MonoBehaviour
{
    [Header("Press this to run the test")]
    public bool testBlips;

    [Header("Speed and Rate")]
    public Text textBox;// reference to the text object to fill with dialog
    public string fullText;// the text to be written out
    public float textRate;// the time to wait between adding characters to the text box
    public float spaceLength;// how long to wait after encountering a space to disable the "Space" parameter of the text event again

    [Header("FMOD")]
    [FMODUnity.EventRef]
    public string BlipEvent;// the name of the FMOD event to play


    private int letterIndex;// how far we are through the string currently
    private FMOD.Studio.EventInstance blipInstance;// the text blip FMOD event

    // Start is called before the first frame update
    void Start()
    {
        blipInstance = FMODUnity.RuntimeManager.CreateInstance(BlipEvent);

        blipInstance.setParameterByName("Space", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(testBlips)
        {
            testBlips = false;
            Reset();
        }
    }

    private void Reset()
    {
        // reset text writing index and text box text
        letterIndex = 0;
        textBox.text = "";

        // reset the FMOD event
        blipInstance.start();
        blipInstance.setParameterByName("Space", 0);

        // reset the repeating event for writing characters to screen
        if(IsInvoking("AdvanceText")) CancelInvoke("AdvanceText");
        InvokeRepeating("AdvanceText", 0, textRate);
    }

    private void AdvanceText()
    {
        if(letterIndex < fullText.Length)// if we're not at the end of the string
        {
            if(fullText[letterIndex] != ' ')
            {
                textBox.text = fullText.Substring(0, letterIndex + 1);
            }
            else
            {
                blipInstance.setParameterByName("Space", 1);
                Invoke("ClearSpace", spaceLength);
            }
            letterIndex++;
        }
        else
        {
            CancelInvoke("AdvanceText");
            blipInstance.setParameterByName("Space", 1);
        }
    }

    // wrapping this behavior in a method so I can call it with Invoke
    private void ClearSpace()
    {
        blipInstance.setParameterByName("Space", 0);
    }


















}
