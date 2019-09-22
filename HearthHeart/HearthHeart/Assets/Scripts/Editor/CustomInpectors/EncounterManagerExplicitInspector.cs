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
                set.numObjects = EditorGUILayout.IntSlider(new GUIContent("# of Objects"), set.numObjects, 0, 6);
                while (set.numObjects > set.objects.Count)
                    set.objects.Add(null);
                while (set.numObjects < set.objects.Count)
                    set.objects.RemoveAt(set.objects.Count - 1);
                for(int i = 0; i < set.objects.Count; ++i)
                {
                    set.objects[i] = EditorUtils.ObjectField(set.objects[i], false);
                }
                return set;
            }
            data.spawnDict.DoGUILayout(ValGUI3, data.spawnDict.EnumAddGUI, "Wave Data", false);
            return data;
        }
        em.data.DoGUILayout(KeyGUI2, ValGUI2, () => em.data.IntAddGUI(ref toAdd), "Data", false);
        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
