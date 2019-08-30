using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.EditorGUIUtils;

[CustomEditor(typeof(EncounterManagerExplicit))]
public class EncounterManagerExplicitInspector : Editor
{
    private enum Op
    {
        None,
        AddAfter,
        Remove,
        MoveUp,
        MoveDown,
    }

    private const float controlBlockWidth = 100;
    private const float controlWidth = controlBlockWidth * 0.25f;
    public int toAdd;
    public override void OnInspectorGUI()
    {
        #region SpawnerGUI
        var em = target as EncounterManagerExplicit;
        SpawnArea ValGUI(SpawnArea obj) => EditorUtils.ObjectField(obj, true);        
        em.spawners.DoGUILayout(ValGUI, em.spawners.EnumAddGUI, "Spawners", true);
        #endregion

        void KeyGUI2(int key) => EditorGUILayout.LabelField("Turn " + key, EditorUtils.Bold);
        WaveData ValGUI2(WaveData data)
        {
            WaveData.SpawnSet ValGUI3(WaveData.SpawnSet set)
            {
                //EditorGUILayout.BeginHorizontal();
                set.obj1 = EditorUtils.ObjectField(set.obj1, false);
                set.obj2 = EditorUtils.ObjectField(set.obj2, false);
                set.obj3 = EditorUtils.ObjectField(set.obj3, false);
                //EditorGUILayout.EndHorizontal();
                return set;
            }
            data.spawnDict.DoGUILayout(ValGUI3, data.spawnDict.EnumAddGUI, "Wave Data", false);
            return data;
        }
        em.data.DoGUILayout(KeyGUI2, ValGUI2, () => em.data.IntAddGUI(ref toAdd), "Data", false);
        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
