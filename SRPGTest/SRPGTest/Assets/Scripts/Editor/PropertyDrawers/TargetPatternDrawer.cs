using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections;

[CustomPropertyDrawer(typeof(TargetPattern))]
public class TargetPatternDrawer : PropertyDrawer
{
    const float labelWidth = 100;
    const float maxCheckWidth = 20;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var maxReachProp = property.FindPropertyRelative("maxReach");
        return (EditorGUIUtility.singleLineHeight + 1) * (3 + (maxReachProp.vector2IntValue.y * 2) + 1);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Initialize properties
        var offsetsProp = property.FindPropertyRelative("offsets");
        var typeProp = property.FindPropertyRelative("type");
        var maxReachProp = property.FindPropertyRelative("maxReach");
        //Initialize UI variables
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

        #region Actual GUI drawing
        GUI.Label(UIRect, new GUIContent("Target Pattern", "TODO, tooltip"), EditorUtils.BoldCentered);
        UIRect.y += lineHeight;
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent("Type"));
        EditorGUI.PropertyField(new Rect(UIRect) { x = UIRect.x + labelWidth, width = UIRect.width - labelWidth }, typeProp, GUIContent.none);
        UIRect.y += lineHeight;
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent("Max Reach"));
        EditorGUI.PropertyField(new Rect(UIRect) { x = UIRect.x + labelWidth, width = UIRect.width - labelWidth }, maxReachProp, GUIContent.none);
        UIRect.y += lineHeight;

        #region Draw Matrix
        var type = (TargetPattern.Type)typeProp.enumValueIndex;
        BoolMatrix2D mat = OffsetArrayPropToBoolMatrix(offsetsProp, maxReachProp.vector2IntValue, type);

        float checkWidth = Mathf.Min(maxCheckWidth, position.width / mat.Rows);
        float checkStartX = position.x + ((position.width - checkWidth * mat.Columns) / 2);
        Rect checkRect = new Rect(UIRect) { width = checkWidth };
        for (int row = 0; row < mat.Rows; ++row)
        {
            checkRect.x = checkStartX;
            for (int col = 0; col < mat.Columns; ++col)
            {
                if (type == TargetPattern.Type.Directional && col == 0 && row == mat.Rows / 2)
                {
                    EditorGUI.LabelField(checkRect, new GUIContent("X"));
                    mat[row, col] = false;
                }          
                else
                    mat[row, col] = EditorGUI.Toggle(checkRect, mat[row, col]);
                checkRect.x += checkWidth;
            }            
            checkRect.y += lineHeight;
        }
        UIRect.y = checkRect.y + lineHeight;
        if(GUI.changed)
            SaveBoolMatrix2DIntoOffsetArrayProp(offsetsProp, mat, maxReachProp.vector2IntValue, type);
        #endregion

        #endregion

        // Set indent back to what it was
        //EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    private BoolMatrix2D OffsetArrayPropToBoolMatrix(SerializedProperty offsetArrayProp, Vector2Int maxReach, TargetPattern.Type type)
    {
        int rows = maxReach.y * 2 + 1;
        int cols = type == TargetPattern.Type.Spread ? maxReach.x * 2 + 1 : maxReach.x + 1;
        var mat = new BoolMatrix2D(rows, cols);
        for(int i = 0; i < offsetArrayProp.arraySize; ++i)
        {
            var offsetProp = offsetArrayProp.GetArrayElementAtIndex(i);
            int col = offsetProp.FindPropertyRelative("col").intValue;
            if (type == TargetPattern.Type.Spread)
                col += maxReach.x;
            int row = offsetProp.FindPropertyRelative("row").intValue + maxReach.y;
            if (mat.Contains(row, col)) 
                mat[row, col] = true;
        }
        return mat;
    }

    private void SaveBoolMatrix2DIntoOffsetArrayProp(SerializedProperty offsetArrayProp, BoolMatrix2D mat, Vector2Int maxReach, TargetPattern.Type type)
    {
        offsetArrayProp.ClearArray();

        for (int row = 0; row < mat.Rows; ++row)
        {
            for (int col = 0; col < mat.Columns; ++col)
            {
                if (mat[row, col])
                {
                    offsetArrayProp.InsertArrayElementAtIndex(0);
                    var offsetProp = offsetArrayProp.GetArrayElementAtIndex(0);
                    offsetProp.FindPropertyRelative("row").intValue = row - maxReach.y;
                    offsetProp.FindPropertyRelative("col").intValue = 
                           type == TargetPattern.Type.Spread ? col - maxReach.x : col;
                }
            }
        }

    }
}
