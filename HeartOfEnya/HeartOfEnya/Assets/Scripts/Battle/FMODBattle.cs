using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODBattle : MonoBehaviour
{
    public static FMODBattle main;

    [SerializeField] private float newWaveLength;

    public bool InEnemyTurn
    {
        set
        {
            float val = value ? 1 : 0;
            enemyTurn.value = val;
            enemyTurn.TriggerParameters();
        }
    }

    public bool TextPlaying
    {
        set
        {
            float val = value ? 1 : 0;
            textPlaying.value = val;
            textPlaying.TriggerParameters();
        }
    }

    public bool NewWave
    {
        set
        {
            float val = value ? 1 : 0;
            newWave.value = val;
            newWave.TriggerParameters();
        }
    }

    public StudioEventEmitter Music
    {
        get
        {
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            //return pData.gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle ? abs0Music : music;

            if (pData.InAbs0Battle)
            {
                return abs0Music;
            }
            else if (pData.InLuaBattle)
            {
                return luacicle;
            }
            else if (pData.LuaUnfrozen)
            {
                return luaUnfrozen;
            }
            else
            {
                return music;
            }
        }
    }

    [Header("Music")]
    [SerializeField]
    private StudioEventEmitter music;
    [SerializeField]
    private StudioEventEmitter abs0Music;
    [SerializeField]
    private StudioEventEmitter luaUnfrozen;
    [SerializeField]
    private StudioEventEmitter luacicle;

    [Header("Ambient")]
    public StudioEventEmitter storm;

    [Header("Effects")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string EnemyTurnTransition;

    [SerializeField]
    [FMODUnity.EventRef]
    private string PlayerTurnTransition;

    [Header("Parameter Triggers")]
    [SerializeField]
    private StudioGlobalParameterTrigger textPlaying;
    [SerializeField]
    private StudioGlobalParameterTrigger enemyTurn;
    [SerializeField]
    private StudioGlobalParameterTrigger newWave;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Music.Play();
    }


    public void TriggerNewWave()
    {
        NewWave = true;
        Invoke("TurnOffNewWave", newWaveLength);
    }

    public void StartPlayerTurn()
    { 
        FMODUnity.RuntimeManager.PlayOneShot(PlayerTurnTransition);
    }

    public void StartEnemyTurn()
    {
        storm.SetParameter("Enemy Turn", 1);
        FMODUnity.RuntimeManager.PlayOneShot(EnemyTurnTransition);
        InEnemyTurn = true;
    }

    public void EndEnemyTurn()
    {
        storm.SetParameter("Enemy Turn", 0);
        InEnemyTurn = false;
    }

    public void EnterCrisis(PartyMember partyMember)
    {
        Music.SetParameter("Crisis", 1);
        Music.SetParameter(partyMember.DisplayName + "Crisis", 1);
    }

    public void PartyMemberOutOfCrisis(PartyMember partyMember)
    {
        Music.SetParameter(partyMember.DisplayName + "Crisis", 0);
    }

    public void ExitCrisis()
    {
        Music.SetParameter("Crisis", 0);
    }

    private void TurnOffNewWave() => NewWave = false;

}
