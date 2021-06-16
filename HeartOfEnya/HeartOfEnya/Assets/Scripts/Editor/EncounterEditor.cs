﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EncounterEditor : EditorWindow
{
    public const string encounterFolderPath = "Assets/ScriptableObjects/Encounters/";
    public const string assetSuffix = ".asset";
    private SpawnPhase spawner = null;
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
    public LevelEditor levelEditor;

    // Wave editing properties
    public Encounter loadedEncounter = null;
    public string newFileName = string.Empty;

    [MenuItem("Window/Level Editor/Encounter Editor")]
    public static void ShowWindow()
    {
        var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        GetWindow<EncounterEditor>("Encounter Editor", typeof(WaveEditor), typeof(LevelEditor), inspectorType);
    }
    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Level editing tools unavailable during play mode.", MessageType.Info);
            return;
        }
        // If there is no grid, we aren't in a battle scene or this one is improperly configured
        if (EditorSceneManager.GetActiveScene().name != LevelEditor.levelEditorSceneName)
        {
            EditorGUILayout.HelpBox("You are not in the LevelEditor scene!", MessageType.Error);
            if(GUILayout.Button(new GUIContent("Go to Level Editor Scene")))
            {
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(LevelEditor.levelEditorScenePath);
                }              
            }
            return;
        }
        if (BattleGrid.main == null)
        {
            EditorGUILayout.HelpBox("No detected battle grid. Please reload scene or add one", MessageType.Error);
            return;
        }
        if(Spawner == null)
        {
            EditorGUILayout.HelpBox("No Spawn Phase. Please reload scene or add one", MessageType.Error);
            return;
        }
        CheckLevelEditor();

        #region Save / Load

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Save / Load Encounter"), EditorUtils.BoldCentered);
        var oldEncounter = loadedEncounter;
        loadedEncounter = EditorUtils.ObjectField(new GUIContent("Loaded Encounter"), loadedEncounter, false);
        if (loadedEncounter != oldEncounter)
        {
            LoadEncounter(loadedEncounter);
        }
        if(loadedEncounter == null)
        {
            EditorGUILayout.HelpBox("No loaded ecnounter. Set the loaded encounter field or use Create New to create a new asset", MessageType.Info);
        }
        else if(GUILayout.Button(new GUIContent("Save")))
        {
            SaveEncounter(loadedEncounter);
        }
        GUILayout.EndVertical();

        #endregion

        #region Create New / Save As Copy

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Copy / Create New Encounter"), EditorUtils.BoldCentered);
        newFileName = EditorGUILayout.TextField(new GUIContent("New File Name"), newFileName);
        if (newFileName == string.Empty)
        {
            EditorGUILayout.HelpBox("New Filename is empty. To create new encounter or save a copy set the New File Name field", MessageType.Info);
        }
        else if (AssetDatabase.LoadAssetAtPath<Encounter>(encounterFolderPath + newFileName + assetSuffix) != null)
        {
            EditorGUILayout.HelpBox("There is already a WaveData asset named " + newFileName + ". Please choose a different name.", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Create New Encounter"))
            {
                var createdEncounter = CreateNewEncounter(newFileName);
                LoadEncounter(createdEncounter);
                newFileName = string.Empty;
            }
            if(loadedEncounter != null)
            {
                if (GUILayout.Button(new GUIContent("Save as Copy")))
                {
                    var createdEncounter = CreateNewEncounter(newFileName);
                    createdEncounter.waveList = new List<WaveData>(loadedEncounter.waveList);
                    EditorUtility.SetDirty(createdEncounter);
                    newFileName = string.Empty;
                }
                if (GUILayout.Button(new GUIContent("Save as Copy + Load")))
                {
                    var createdEncounter = CreateNewEncounter(newFileName);
                    createdEncounter.waveList = new List<WaveData>(loadedEncounter.waveList);
                    EditorUtility.SetDirty(createdEncounter);
                    LoadEncounter(createdEncounter);
                    newFileName = string.Empty;
                }
            }
        }
        GUILayout.EndVertical();

        #endregion

        #region Encounter Property Editor

        if (loadedEncounter != null)
        {
            EditEncounterProperties(loadedEncounter);
        }

        #endregion

    }

    public void CheckLevelEditor()
    {
        if (levelEditor == null)
        {
            levelEditor = GetWindow<LevelEditor>("Level Editor", typeof(EncounterEditor));
        }
    }

    #region Helper Methods

    void EditEncounterProperties(Encounter encounter)
    {
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(new GUIContent("Encounter Properties"), EditorUtils.BoldCentered);
        var editor = Editor.CreateEditor(encounter);
        editor.OnInspectorGUI();
        GUILayout.EndVertical();
    }

    Encounter CreateNewEncounter(string name)
    {
        var encounter = ScriptableObject.CreateInstance<Encounter>();
        AssetDatabase.CreateAsset(encounter, encounterFolderPath + name + assetSuffix);
        return encounter;
    }

    void LoadEncounter(Encounter encounter)
    {
        loadedEncounter = encounter;
        Undo.RecordObject(Spawner, "Set active encounter");
        Spawner.levelEditorEncounter = encounter;
        PrefabUtility.RecordPrefabInstancePropertyModifications(Spawner);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void SaveEncounter(Encounter encounter)
    {
        EditorUtility.SetDirty(encounter);
    }

    #endregion
}
