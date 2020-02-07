using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
    public const string folderPath = "Assets/ScriptableObjects/WaveData/";
    public const string assetSuffix = ".asset";
    public WaveData loadedWave = null;
    public string newFileName = string.Empty;
    private Scene activeScene;
    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelEditor));
    }
    private void OnGUI()
    {
        // If there is no grid, we aren't in a battle scene or this one is improperly configured
        if (BattleGrid.main == null)
        {
            loadedWave = null;
            EditorGUILayout.HelpBox("No detected battle grid. If you aren't in a battle scene, go to one. If you are in a battle scene, please reload scene or add one", MessageType.Error);
            return;
        }

        // Null out loaded wave on scene change
        var oldScene = activeScene;
        activeScene = EditorSceneManager.GetActiveScene();
        if (oldScene != activeScene)
            loadedWave = null;

        #region Saving / Loading

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Saving and Loading"), EditorUtils.BoldCentered);
        var oldWave = loadedWave;
        loadedWave = EditorUtils.ObjectField(new GUIContent("Loaded Wave"), loadedWave, false);
        if(oldWave != loadedWave)
        {
            if (loadedWave != null)
                LoadWave(loadedWave);
            else
                ClearEnemies();
        }
        if(loadedWave == null)
        {
            EditorGUILayout.HelpBox("No loaded wave. Set the loaded wave field or use Save As to create a new asset", MessageType.Info);
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
        else if(AssetDatabase.LoadAssetAtPath<WaveData>(folderPath + newFileName + assetSuffix) != null)
        {
            EditorGUILayout.HelpBox("There is already a WaveData asset named " + newFileName + ". Please choose a different name.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Save As New")))
            {
                var newWave = CreateNew(newFileName);
                SaveWave(newWave);
                newFileName = string.Empty;
            }
            if (GUILayout.Button(new GUIContent("Save As New + Load")))
            {
                var newWave = CreateNew(newFileName);
                SaveWave(newWave);
                loadedWave = newWave;
                newFileName = string.Empty;
            }

        }
        GUILayout.EndVertical();

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
        if (GUILayout.Button(new GUIContent("Clear Enemies")))
        {
            ClearEnemies();
        }
        GUILayout.EndVertical();

        #endregion
    }

    WaveData CreateNew(string name)
    {
        var wave = ScriptableObject.CreateInstance<WaveData>();
        AssetDatabase.CreateAsset(wave, folderPath + name + assetSuffix);
        return wave;
    }

    void LoadWave(WaveData wave)
    {
        ClearEnemies();
        var parent = GameObject.Find("Enemies");
        foreach (var spawn in wave.data)
        {
            Object newObj;
            if (parent == null)
                newObj = PrefabUtility.InstantiatePrefab(spawn.enemy);
            else
                newObj = PrefabUtility.InstantiatePrefab(spawn.enemy, parent.transform);
            Undo.RegisterCreatedObjectUndo(newObj, "Created " + newObj.name);
            var enemy = (newObj as GameObject).GetComponent<Enemy>();
            Undo.RecordObject(enemy, enemy.name);
            enemy.Pos = spawn.spawnPosition;
            enemy.transform.position = BattleGrid.main.GetSpace(enemy.Pos);
        }
    }

    void ClearEnemies()
    {
        var enemies = Object.FindObjectsOfType<Enemy>();
        Undo.RecordObjects(enemies, "Clear Enemies");
        foreach (var obj in enemies)
        {
            DestroyImmediate(obj.gameObject);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void SaveWave(WaveData wave)
    {
        GraphPosition.PositionFieldObjectsGraphSpace();
        var enemies = Object.FindObjectsOfType<Enemy>();
        Undo.RecordObject(wave, wave.name);
        wave.data.Clear();
        foreach (var obj in enemies)
        {
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            wave.data.Add(new WaveData.SpawnData
            {
                spawnPosition = obj.Pos,
                enemy = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject
            });
        }
        EditorUtility.SetDirty(wave);
    }
}
