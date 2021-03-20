using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public bool goToSoup = true;
    public Encounter mainEncounter = null;
    public FMODUnity.StudioEventEmitter selectSfx;
    public GenericConfirmationMenu confirmationPrompt;
    private Button[] buttons;

    private void Start()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        var pSaveData = DoNotDestroyOnLoad.Instance.permanentSaveData;
        string latestPhase = pSaveData.LatestGamePhase;
        int latestPhaseInt = PersistentData.GamePhaseToInt(latestPhase);
        int latestDay = pSaveData.LatestDay;
        bool Disable(int phase, int day, bool camp)
        {
            if (phase == latestPhaseInt)
            {
                if (latestPhase == PersistentData.gamePhaseTut3AndLuaBattle && latestDay > 0 && day > 0)
                    return pSaveData.OnBattle && camp;
                if (latestPhase == PersistentData.gamePhaseAbsoluteZeroBattle)
                    return pSaveData.OnBattle && camp;
                if (day == latestDay)
                    return pSaveData.OnBattle && camp;
                return day > latestDay;
            }
            return phase > latestPhaseInt;
        }
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length - 1; ++i)
        {
            var button = buttons[i];
            // Why have i committed this crime against indices
            int phase = (i + 3) / 4;
            int day = ((i - 1) % 4) / 2;
            bool camp = i % 2 == 0;
            if (Disable(phase, day, camp))
            {
                button.GetComponentInChildren<Text>().text = "???";
                continue;
            }
            button.interactable = true;
            if (camp) // Button goes to camp scene
            {
                button.onClick.AddListener(() => GoToCamp(PersistentData.IntToGamePhase(phase), day));
            }
            else // Button goes to battle / soup scene
            {
                button.onClick.AddListener(() => GoToBattle(PersistentData.IntToGamePhase(phase), day));
            }
            button.onClick.AddListener(() => selectSfx.Play());
        }
        var creditsButton = buttons[buttons.Length - 1];
        creditsButton.onClick.AddListener(() => SceneTransitionManager.main.TransitionScenes("Credits"));
        creditsButton.onClick.AddListener(() => selectSfx.Play());
    }

    public void SetGoToSoup(bool b) => goToSoup = b;

    public void GoToBattle(string phase, int dayNum)
    {
        confirmationPrompt.OnConfirm = null;
        confirmationPrompt.OnConfirm += () =>
        {
            SetPersistantData(phase, dayNum);
            if (goToSoup && !(phase == PersistentData.gamePhaseTut1And2 && dayNum == PersistentData.dayNumStart))
            {
                SceneTransitionManager.main.TransitionScenes("Breakfast");
            }
            else
            {
                SceneTransitionManager.main.TransitionScenes("Battle");
            }
        };
        confirmationPrompt.Show();
    }

    public void GoToCamp(string phase, int dayNum)
    {
        confirmationPrompt.OnConfirm = null;
        confirmationPrompt.OnConfirm += () =>
        {
            SetPersistantData(phase, dayNum);
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            if (phase == PersistentData.gamePhaseIntro)
            {
                SceneTransitionManager.main.TransitionScenes("IntroCamp");
            }
            else if (phase == PersistentData.gamePhaseAbsoluteZeroBattle)
            {
                pData.absoluteZeroPhase1Defeated = true;
                SceneTransitionManager.main.TransitionScenes("OutroCamp");
            }
            else
            {
                if (pData.InLuaBattle)
                    pData.luaBossPhase2Defeated = true;
                SceneTransitionManager.main.TransitionScenes("Camp");
            }
        };
        confirmationPrompt.Show();
    }

    private void SetPersistantData(string phase, int dayNum)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.dayNum = dayNum;
        pData.gamePhase = phase;
        if (pData.InMainPhase || pData.LuaUnfrozen)
        {
            pData.luaBossPhase2Defeated = true;
            pData.lastEncounter = mainEncounter;
            if(phase == PersistentData.gamePhaseBeginMain && dayNum == PersistentData.dayNumStart + 1)
            {
                pData.waveNum = 4;
            }
            else if(phase == PersistentData.gamePhaseLuaUnfrozen)
            {
                pData.waveNum = dayNum == PersistentData.dayNumStart ? 8 : 12;
            }
            else if(phase == PersistentData.gamePhaseAbsoluteZeroBattle)
            {
                pData.waveNum = 16;
            }
            int totalEnemies = 0;
            for (int i = mainEncounter.Waves.Length - 1; i >= pData.waveNum; --i)
            {
                totalEnemies += mainEncounter.Waves[i].enemies.Count;
            }
            pData.numEnemiesLeft = totalEnemies;
        }   
    }
}
