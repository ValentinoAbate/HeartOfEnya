using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
    public const string levelEditorSceneName = "LevelEditor";
    public const string levelEditorScenePath = "Assets/Scenes/LevelEditor/" + levelEditorSceneName + ".unity";
    public enum PlayMode
    { 
        PlayWave,
        PlayEncounter,
    }
    public PlayMode playMode;
    public int startEncounterAtWave = 0;
    public int partyLevel = 1;
    public bool enableLua = false;
    public WaveEditor waveEditor;
    public EncounterEditor encounterEditor;
    public GameObject obstacleContainer;
    public GameObject enemyContainer;

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
    public PersistentData persistentData;
    public PersistentData PersistantData
    {
        get
        {
            if (persistentData == null)
            {
                persistentData = Object.FindObjectOfType<PersistentData>();
            }
            return persistentData;
        }
    }


    [MenuItem("Window/Level Editor/Level Editor")]
    public static void ShowWindow()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        var window = GetWindow<LevelEditor>("Level Editor", inspectorType);
        window.RefreshReferences();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void EnactPlayModeSettings(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            if(playMode == PlayMode.PlayEncounter)
            {
                foreach (var enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy.transform.parent != EnemyContainer.transform)
                        enemy.transform.SetParent(EnemyContainer.transform);
                    enemy.gameObject.SetActive(false);
                }
                foreach (var obstacle in FindObjectsOfType<Obstacle>())
                {
                    if (obstacle.transform.parent != ObstacleContainer.transform)
                        obstacle.transform.SetParent(ObstacleContainer.transform);
                    obstacle.gameObject.SetActive(false);
                }
                Spawner.spawnEnemies = true;
            }
            else
            {
                Spawner.spawnEnemies = false;
            }
        }
        else if(state == PlayModeStateChange.EnteredEditMode)
        {
            if (playMode == PlayMode.PlayEncounter)
            {
                foreach (var enemy in EnemyContainer.GetComponentsInChildren<Enemy>(true))
                {
                    enemy.gameObject.SetActive(true);
                }                
                foreach (var obstacle in ObstacleContainer.GetComponentsInChildren<Obstacle>(true))
                {
                    obstacle.gameObject.SetActive(true);
                }
            }
            else
            {
                Spawner.spawnEnemies = true;
            }
        }
    }

    private void SetupListener(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        if (scene.name == "LevelEditor")
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
        EditorSceneManager.sceneOpened -= SetupListener;
        EditorSceneManager.sceneOpened += SetupListener;
        if (EditorSceneManager.GetActiveScene().name != levelEditorSceneName)
            return;
        EditorApplication.playModeStateChanged -= EnactPlayModeSettings;
        EditorApplication.playModeStateChanged += EnactPlayModeSettings;
    }

    public void RefreshReferences()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        waveEditor = GetWindow<WaveEditor>("Wave Editor", typeof(LevelEditor), inspectorType);
        encounterEditor = GetWindow<EncounterEditor>("Encounter Editor", typeof(WaveEditor), typeof(LevelEditor), inspectorType);
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Level editing tools unavailable during play mode.", MessageType.Info);
            return;
        }
        // If there is no grid, we aren't in a battle scene or this one is improperly configured
        if (EditorSceneManager.GetActiveScene().name != levelEditorSceneName)
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
        if (waveEditor == null || encounterEditor == null)
            RefreshReferences();
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Player Properties"), EditorUtils.BoldCentered);
        var oldPlayMode = playMode;
        playMode = EditorUtils.EnumPopup(new GUIContent("Play Mode"), playMode);
        if(playMode != oldPlayMode)
        {
            Undo.RecordObject(Spawner, "Set playMode");
            Spawner.spawnEnemies = playMode == PlayMode.PlayEncounter;
            PrefabUtility.RecordPrefabInstancePropertyModifications(Spawner);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if(playMode == PlayMode.PlayEncounter)
        {           
            if(encounterEditor.loadedEncounter == null)
            {
                EditorGUILayout.HelpBox("No Loaded Encounter. Use the Encounter Editor To load one", MessageType.Error);
            }
            else
            {
                int oldWaveStart = startEncounterAtWave;
                startEncounterAtWave = EditorGUILayout.IntSlider(new GUIContent("Start At Wave"), startEncounterAtWave, 1, encounterEditor.loadedEncounter.waveList.Count);
                if (startEncounterAtWave > encounterEditor.loadedEncounter.waveList.Count)
                    startEncounterAtWave = 1;
                if(startEncounterAtWave != oldWaveStart)
                {
                    Undo.RecordObject(Spawner, "Set active encounter");
                    Spawner.startAtWave = startEncounterAtWave;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(Spawner);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }                   
            }
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Party Properties"), EditorUtils.BoldCentered);
        int oldPartyLevel = partyLevel;
        partyLevel = EditorGUILayout.IntSlider(new GUIContent("Party Level"), partyLevel, 1, 4);
        if (partyLevel != oldPartyLevel)
        {
            Undo.RecordObject(PersistantData, "Set party level");
            PersistantData.levelEditorPartyLevel = partyLevel;
            PrefabUtility.RecordPrefabInstancePropertyModifications(PersistantData);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        bool oldLuaEnabled = enableLua;
        enableLua = EditorGUILayout.Toggle(new GUIContent("Enable Lua"), enableLua);
        if (enableLua != oldLuaEnabled)
        {
            Undo.RecordObject(Spawner, "Set lua override");
            Spawner.overrideSpawnLua = enableLua;
            PrefabUtility.RecordPrefabInstancePropertyModifications(Spawner);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        GUILayout.EndVertical();
    }
}
