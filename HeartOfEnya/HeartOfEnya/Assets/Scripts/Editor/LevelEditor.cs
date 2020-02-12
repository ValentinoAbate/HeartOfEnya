using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
    public const string levelEditorScenePath = "Assets/Scenes/LevelEditor/LevelEditor.unity";
    public enum PlayMode
    { 
        PlayWave,
        PlayEncounter,
    }
    public PlayMode playMode;
    public int startEncounterAtWave = 0;
    public WaveEditor waveEditor;
    public EncounterEditor encounterEditor;
    public GameObject obstacleContainer;
    public GameObject enemyContainer;
    private bool initialized = false;

    public GameObject EnemyContainer 
    {
        get
        {
            if(enemyContainer == null)
                enemyContainer = GameObject.Find("Enemies");
            return enemyContainer;
        }
    }
    public GameObject ObstacleContainer
    {
        get
        {
            if (obstacleContainer == null)
                obstacleContainer = GameObject.Find("Obstacles");
            return obstacleContainer;
        }
    }
    public SpawnPhase spawner;
    public SpawnPhase Spawner
    {
        get
        {
            if (spawner == null)
            {
                spawner = Object.FindObjectOfType<SpawnPhase>();
            }
            return spawner;
        }
    }

    [MenuItem("Window/Level Editor/Level Editor")]
    public static void ShowWindow()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        var window = GetWindow<LevelEditor>("Level Editor", inspectorType);
        window.RefreshReferences();
        window.Initialize();
    }

    private void EnactPlayModeSettings(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            if(playMode == PlayMode.PlayEncounter)
            {
                foreach (Transform enemy in EnemyContainer.transform)
                {
                    enemy.gameObject.SetActive(false);
                }
                foreach (Transform obstacle in ObstacleContainer.transform)
                {
                    obstacle.gameObject.SetActive(false);
                }
                Spawner.enabled = true;
            }
            else
            {
                Spawner.enabled = false;
            }
        }
        else if(state == PlayModeStateChange.EnteredEditMode)
        {
            if (playMode == PlayMode.PlayEncounter)
            {
                foreach (Transform enemy in EnemyContainer.transform)
                {
                    enemy.gameObject.SetActive(true);
                }                
                foreach (Transform obstacle in ObstacleContainer.transform)
                {
                    obstacle.gameObject.SetActive(true);
                }
            }
            else
            {
                Spawner.enabled = true;
            }
            Initialize();
        }
    }

    private void SetupListener(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {        
        if(arg1.name == "LevelEditor")
        {
            EditorApplication.playModeStateChanged += EnactPlayModeSettings;
        }
        else
        {
            EditorApplication.playModeStateChanged -= EnactPlayModeSettings;
        }
    }

    public void Initialize()
    {
        if (initialized)
            return;
        EditorSceneManager.activeSceneChangedInEditMode -= SetupListener;
        EditorSceneManager.activeSceneChangedInEditMode += SetupListener;
        EditorApplication.playModeStateChanged += EnactPlayModeSettings;
        initialized = true;
    }

    public void RefreshReferences()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        waveEditor = GetWindow<WaveEditor>("Wave Editor", typeof(LevelEditor), inspectorType);
        encounterEditor = GetWindow<EncounterEditor>("Encounter Editor", typeof(WaveEditor), typeof(LevelEditor), inspectorType);
    }

    private void OnGUI()
    {
        // If there is no grid, we aren't in a battle scene or this one is improperly configured
        if(EditorSceneManager.GetActiveScene().name != "LevelEditor")
        {
            EditorGUILayout.HelpBox("You are not in the LevelEditor scene!", MessageType.Error);
            if(GUILayout.Button(new GUIContent("Go to Level Editor Scene")))
            {
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(levelEditorScenePath);
                }              
            }
            return;
        }
        Initialize();
        if (waveEditor == null || encounterEditor == null)
            RefreshReferences();
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Player Properties"), EditorUtils.BoldCentered);
        playMode = EditorUtils.EnumPopup(new GUIContent("Play Mode"), playMode);
        if(playMode == PlayMode.PlayEncounter)
        {           
            if(encounterEditor.loadedEncounter == null)
            {
                EditorGUILayout.HelpBox("No Loaded Encounter. Use the Encounter Editor To load one", MessageType.Error);
            }
            else
            {
                int oldVal = startEncounterAtWave;
                startEncounterAtWave = EditorGUILayout.IntSlider(new GUIContent("Start At Wave"), startEncounterAtWave, 1, encounterEditor.loadedEncounter.waveList.Count);
                if (startEncounterAtWave > encounterEditor.loadedEncounter.waveList.Count)
                    startEncounterAtWave = 1;
                if(startEncounterAtWave != oldVal)
                {
                    Undo.RecordObject(Spawner, "Set active encounter");
                    Spawner.startAtWave = startEncounterAtWave;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(Spawner);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }                   
            }
        }
        GUILayout.EndVertical();
    }
}
