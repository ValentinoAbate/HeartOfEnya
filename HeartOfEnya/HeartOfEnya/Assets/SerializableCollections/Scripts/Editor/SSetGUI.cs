using UnityEngine;
using UnityEditor;

namespace SerializableCollections.EditorGUIUtils
{
    using Utilities;
    public static class SSetGUI
    {
        public delegate void DisplayGUI<T>(T value);
        public delegate void AddGUI();

        public static void StringAddGUI(SSet<string> set, ref string toAdd)
        {
            EditorGUILayout.LabelField("New:", GUILayout.Width(45));
            toAdd = EditorGUILayout.TextField(toAdd);
            if (GUILayout.Button(new GUIContent("Add"), GUILayout.Width(45)))
            {
                if (!string.IsNullOrWhiteSpace(toAdd) && !set.Contains(toAdd))
                {
                    set.Add(toAdd);
                }
                toAdd = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
        }

        public static void EnumAddGUI<T>(this SSet<T> set) where T : System.Enum
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("+"), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var t in EnumUtils.GetValues<T>())
                    if (!set.Contains(t))
                        menu.AddItem(new GUIContent(t.ToString()), false, (obj) => set.Add((T)obj), t);
                menu.ShowAsContext();
            }
        }

        public static void ObjPickerAddGUI<T>(this SSet<T> set, string filter = "", bool allowSceneObjects = false) where T : UnityEngine.Object
        {
            Event e = Event.current;
            if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
            {
                T item = EditorGUIUtility.GetObjectPickerObject() as T;
                if (item == null)
                    return;
                set.Add(item);
                e.Use();
                return;
            }
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<T>(null, allowSceneObjects, filter, 1);
        }

        public static void GenericMenuAddGUI<T>(this SSet<T> set, GenericMenu menu)
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("+"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
        }

        public static void DoGUILayout<T>(this SSet<T> set, DisplayGUI<T> display, AddGUI addGUI, string title)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ": " + set.Count, EditorUtils.Bold, GUILayout.MaxWidth(120));
            GUILayout.Space(-20);
            //GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear"))
                set.Clear();
            addGUI();
            GUILayout.EndHorizontal();
            //EditorUtils.Separator();
            if (set.Count > 0)
                DoGUILayout(set, display);
        }

        private static void DoGUILayout<T>(SSet<T> set, DisplayGUI<T> display)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            T toDelete = default;
            bool delete = false;
            T[] items = new T[set.Count];
            set.Items.CopyTo(items, 0);
            System.Array.Sort(items);
            foreach (var item in items)
            {
                GUILayout.BeginHorizontal();
                display(item);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(45)))
                {
                    toDelete = item;
                    delete = true;
                }
                GUILayout.EndHorizontal();
            }
            if (delete)
                set.Remove(toDelete);
            EditorGUI.indentLevel--;
            //EditorUtils.Separator();
            EditorGUILayout.EndVertical();
        }
    }
}
