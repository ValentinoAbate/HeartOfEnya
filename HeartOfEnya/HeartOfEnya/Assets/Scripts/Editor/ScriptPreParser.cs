using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

public class ScriptPreParser : EditorWindow
{
    public TextAsset script;

    [MenuItem("Window/Dialog Parser")]
    public static void ShowWindow()
    {
        //var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        GetWindow<ScriptPreParser>("Dialog Parser");//, inspectorType);
    }

    private void OnGUI()
    {
        script = EditorUtils.ObjectField(new GUIContent("Script"), script, false);
        if (script == null)
        {
            EditorGUILayout.HelpBox("No loaded script. Set the script field", MessageType.Info);
        }
        else if (GUILayout.Button(new GUIContent("Pre-Parse")))
        {
            string oldText = script.text;         
            string newText = Regex.Replace(script.text, @"\[\w*\]", string.Empty);
            if(oldText != newText)
            {
                File.WriteAllText(AssetDatabase.GetAssetPath(script), newText);
                EditorUtility.SetDirty(script);
            }
        }
    }
}
