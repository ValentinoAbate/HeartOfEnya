using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class SoupEvents : MonoBehaviour
{
    // struct wrapper for unity event containing flag
    [System.Serializable]
    public struct SoupEvent
    {
        public UnityEvent _event;
        public bool flag;
    }

    public static SoupEvents main;
    [HideInInspector] public bool tutorialDay1;

    [Header("Tutorial Events")]
    public SoupEvent tutorialIntro;

    //private Canvas tutorialCanvas;

    private void Awake()
    {
        Debug.Log("AWAKE");
        if (main == null)
        {
            main = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        //tutorialCanvas = transform.Find("TutorialCanvas").GetComponent<Canvas>();
        //tutorialCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        //tutorialCanvas.renderMode = RenderMode.WorldSpace;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        tutorialDay1 = pData.InTutorialFirstDay;
        Debug.Log("EVENTS LOADING " + tutorialDay1);
    }

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IntroTrigger()
    {
        Debug.Log("TRIGGERED$$$$$$$$$$$$$$$$$$");
        if(tutorialDay1 && !tutorialIntro.flag)
        {
            Debug.LogWarning("Soup Triggers: start of soup");
            tutorialIntro.flag = true;
            //BattleUI.main.HideEndTurnButton(true);

            // Start the dialog (connect to ambers code)
            //Pause();
            //DialogManager.main.runner.StartDialogue("TutIntro");
            
            // Wait for finish StartCoroutine(IntroTriggerPost(runner))
            //StartCoroutine(IntroTriggerPost(DialogManager.main.runner));
        }
    }

    // private IEnumerator IntroTriggerPost(DialogueRunner runner)
    // {
    //     //yield return new WaitWhile(() => runner.isDialogueRunning);

    //     // post-condition: disable everyone but raina
    //     //string[] units = {"Bapy", "Soleil"};
    //     //partyPhase.DisableUnits(new List<string>(units));
    //     // restrict raina's movement to specific square
    //     //BattleUI.main.MoveableTiles.Add(rainaMovePos);
    //     //BattleUI.main.ShowPrompt("Click on Raina to select them.");
    //     //Unpause();        
    // }
}
