using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public bool goToSoup = true;
    private Button[] buttons;

    private void Start()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        buttons = GetComponentsInChildren<Button>();
        for(int i = 0; i < buttons.Length; ++i)
        {
            var button = buttons[i];
            int phase = (i + 3) / 4;
            int day = ((i - 1) % 4) / 2;
            if(i % 2 == 0) // Button goes to camp scene
            {
                button.onClick.AddListener(() => GoToCamp(pData.IntToGamePhase(phase), day));
            }
            else // Button goes to battle / soup scene
            {
                button.onClick.AddListener(() => GoToBattle(pData.IntToGamePhase(phase), day));
            }
        }
    }

    public void SetGoToSoup(bool b) => goToSoup = b;

    public void GoToBattle(string phase, int dayNum)
    {
        SetPersistantData(phase, dayNum);
        if(goToSoup)
        {
            SceneTransitionManager.main.TransitionScenes("Breakfast");
        }
        else
        {
            SceneTransitionManager.main.TransitionScenes("Battle");
        }
    }

    public void GoToCamp(string phase, int dayNum)
    {
        SetPersistantData(phase, dayNum);
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (phase == PersistentData.gamePhaseIntro)
        {
            SceneTransitionManager.main.TransitionScenes("IntroCamp");
        }
        else if(phase == PersistentData.gamePhaseAbsoluteZeroBattle)
        {
            pData.absoluteZeroDefeated = true;
            SceneTransitionManager.main.TransitionScenes("OutroCamp");
        }
        else
        {
            if (pData.InLuaBattle)
                pData.luaBossDefeated = true;
            SceneTransitionManager.main.TransitionScenes("Camp");
        }
    }

    private void SetPersistantData(string phase, int dayNum)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.dayNum = dayNum;
        pData.gamePhase = phase;
        if (pData.InMainPhase)
            pData.luaBossDefeated = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
