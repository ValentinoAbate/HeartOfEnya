using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class WaveEditor : EditorWindow
{
    public const string waveFolderPath = "Assets/ScriptableObjects/WaveData/";
    public const string assetSuffix = ".asset";
    public const string levelEditorScenePath = "Assets/Scenes/LevelEditor/LevelEditor.unity";
    // Wave editing properties
    public WaveData loadedWave = null;
    public string newFileName = string.Empty;
    public LevelEditor levelEditor;

    [MenuItem("Window/Level Editor/Wave Editor")]
    public static void ShowWindow()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        GetWindow<WaveEditor>("Wave Editor", typeof(LevelEditor), inspectorType);
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
        if (BattleGrid.main == null)
        {
            EditorGUILayout.HelpBox("No detected battle grid. Please reload scene or add one", MessageType.Error);
            return;
        }
        CheckLevelEditor();

        #region Wave Saving / Loading

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Save / Load Wave"), EditorUtils.BoldCentered);
        var oldWave = loadedWave;
        loadedWave = EditorUtils.ObjectField(new GUIContent("Loaded Encounter"), loadedWave, false);
        if(oldWave != loadedWave)
        {
            if (loadedWave != null)
                LoadWave(loadedWave);
            else
                ClearObjects();
        }
        if(loadedWave == null)
        {
            EditorGUILayout.HelpBox("No loaded wave. Set the loaded wave field or use Save as New to create a new asset", MessageType.Info);
        }
        else if(GUILayout.Button(new GUIContent("Save")))
        {
            SaveWave(loadedWave);
        }
        GUILayout.EndVertical();

        #endregion

        #region Save as New Wave

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Save as New Wave"), EditorUtils.BoldCentered);
        newFileName = EditorGUILayout.TextField(new GUIContent("New File Name"), newFileName);
        if(newFileName == string.Empty)
        {
            EditorGUILayout.HelpBox("New Filename is empty. To save as new wave asset set the New File Name field", MessageType.Info);
        }
        else if(AssetDatabase.LoadAssetAtPath<WaveData>(waveFolderPath + newFileName + assetSuffix) != null)
        {
            EditorGUILayout.HelpBox("There is already a WaveData asset named " + newFileName + ". Please choose a different name.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Save as New")))
            {
                var newWave = CreateNewWave(newFileName);              
                SaveWave(newWave);
                newFileName = string.Empty;
            }
            if (GUILayout.Button(new GUIContent("Save as New + Load")))
            {
                var newWave = CreateNewWave(newFileName);
                SaveWave(newWave);
                loadedWave = newWave;
                newFileName = string.Empty;
            }

        }
        GUILayout.EndVertical();

        #endregion

        #region Wave Property Editor

        if(loadedWave != null)
        {
            EditWaveProperties(loadedWave);
        }

        #endregion

        #region Grid Utilities

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Grid Utilities"), EditorUtils.BoldCentered);
        if(GUILayout.Button(new GUIContent("Set All Unit Grid Positions")))
        {
            GraphPosition.PositionFieldObjectsGraphSpace();
        }
        if (GUILayout.Button(new GUIContent("Set All Unit World Positions")))
        {
            GraphPosition.PositionFieldObjectsWorldSpace();
        }
        if (GUILayout.Button(new GUIContent("Clear Objects")))
        {
            ClearObjects();
        }
        GUILayout.EndVertical();

        #endregion

    }

    public void CheckLevelEditor()
    {
        if(levelEditor == null)
        {
            levelEditor = GetWindow<LevelEditor>("Level Editor", typeof(WaveEditor));
        }
        levelEditor.Initialize();
    }

    #region Helper Methods

    void EditWaveProperties(WaveData wave)
    {
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Wave Properties"), EditorUtils.BoldCentered);
        EditorGUI.BeginChangeCheck();
        wave.spawnAfterTurns = EditorGUILayout.ToggleLeft(new GUIContent("Spawn Next Wave After Turns"), wave.spawnAfterTurns);
        if (wave.spawnAfterTurns)
            wave.numTurns = EditorGUILayout.IntField(new GUIContent("Number of Turns"), wave.numTurns);
        wave.spawnWhenNumberOfEnemiesRemain = EditorGUILayout.ToggleLeft(new GUIContent("Spawn Next Wave When Number of Enemies Remain"), wave.spawnWhenNumberOfEnemiesRemain);
        if (wave.spawnWhenNumberOfEnemiesRemain)
            wave.numEnemies = EditorGUILayout.IntField(new GUIContent("Number of Enemies"), wave.numEnemies);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(wave);
        }
        GUILayout.EndVertical();
    }

    WaveData CreateNewWave(string name)
    {
        var wave = ScriptableObject.CreateInstance<WaveData>();
        AssetDatabase.CreateAsset(wave, waveFolderPath + name + assetSuffix);
        return wave;
    }

    void LoadWave(WaveData wave)
    {
        ClearObjects();
        var parent = GameObject.Find("Enemies");
        LoadObjects(wave.enemies, parent);
        parent = GameObject.Find("Obstacles");
        LoadObjects(wave.obstacles, parent);
    }

    private void LoadObjects(List<WaveData.SpawnData> spawns, GameObject parent)
    {
        foreach (var spawn in spawns)
        {
            Object newObj;
            if (parent == null)
                newObj = PrefabUtility.InstantiatePrefab(spawn.spawnObject);
            else
                newObj = PrefabUtility.InstantiatePrefab(spawn.spawnObject, parent.transform);
            Undo.RegisterCreatedObjectUndo(newObj, "Created " + newObj.name);
            var fieldObj = (newObj as GameObject).GetComponent<FieldObject>();
            Undo.RecordObject(fieldObj, fieldObj.name);
            fieldObj.Pos = spawn.spawnPosition;
            fieldObj.transform.position = BattleGrid.main.GetSpace(fieldObj.Pos);
        }
    }

    void ClearObjects()
    {
        ClearObjects<Enemy>();
        ClearObjects<Obstacle>();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void ClearObjects<T>() where T : FieldObject
    {
        var objects = Object.FindObjectsOfType<T>();
        Undo.RecordObjects(objects, "Clear Objects");
        foreach (var obj in objects)
        {
            DestroyImmediate(obj.gameObject);
        }
    }

    void SaveWave(WaveData wave)
    {
        GraphPosition.PositionFieldObjectsGraphSpace();
        Undo.RecordObject(wave, wave.name);
        SaveObjects<Enemy>(wave.enemies);
        SaveObjects<Obstacle>(wave.obstacles);
        EditorUtility.SetDirty(wave);
    }
    
    /// <summary>
    /// Helper method to record different types of field objects in a wave data
    /// </summary>
    private void SaveObjects<T>(List<WaveData.SpawnData> container) where T : FieldObject
    {
        container.Clear();
        var objects = Object.FindObjectsOfType<T>();
        foreach (var obj in objects)
        {
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            container.Add(new WaveData.SpawnData
            {
                spawnPosition = obj.Pos,
                spawnObject = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject
            });
        }
    }

    #endregion
}
