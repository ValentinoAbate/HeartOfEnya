using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleEvents : MonoBehaviour
{
    // struct wrapper for unity event containing flag
    [System.Serializable]
    public struct BattleEvent
    {
        public UnityEvent _event;
        public bool flag;
    }

    public static BattleEvents main;
    public bool tutorial; // whether we are in the tutorial or not

    public BattleEvent tutorialIntro;
    public BattleEvent tutMove;
    public BattleEvent tutRainaAttack;
    public BattleEvent tutBapySelect;
    public BattleEvent tutBapyCancel;
    public BattleEvent tutSoleilSelect;
    public BattleEvent tutSoleilAttack;
    public BattleEvent tutSoleilChargeReminder;
    public BattleEvent tutSoleilChargeExplanation;
    public BattleEvent tutEnemySpawnWarning;
    public BattleEvent tutEnemySpawn;
    public BattleEvent tutEnemyInfo;
    public BattleEvent tutEnemyRanged;
    public BattleEvent tutDD;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }

        tutorial = (DoNotDestroyOnLoad.Instance.persistentData.gamePhase == PersistentData.gamePhaseTutorial);
    }

    public void IntroTrigger()
    {
        if(!tutorialIntro.flag)
        {
            Debug.Log("start of battle");
            tutorialIntro.flag = true;
        }
    }


    public void MoveTrigger()
    {

    }

    public void RainaAttackTrigger()
    {
        
    }

    public void BapySelectTrigger()
    {
        
    }

    public void BapyCancelTrigger()
    {
        
    }

    public void SoleilSelectTrigger()
    {
        
    }

    public void SoleilAttackTrigger()
    {
        
    }

    public void SoleilChargeReminderTrigger()
    {
        
    }

    public void SoleilChargeExplanationTrigger()
    {
        
    }

    public void EnemySpawnWarningTrigger()
    {

    }

    public void EnemySpawnTrigger()
    {

    }

    public void EnemyInfoTrigger()
    {

    }

    public void EnemyRangedTrigger()
    {

    }

    public void DeathsDoorTrigger()
    {

    }
}
