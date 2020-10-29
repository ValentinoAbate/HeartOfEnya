using UnityEngine;
using UnityEditor;

namespace SerializableCollections.EditorGUIUtils
{
    using Utilities;
    public static class SDictionaryGUI
    {
        public delegate T ValueGUI<T>(T value);
        public delegate void KeyGUI<T>(T value);
        public delegate void AddGUI();
        public delegate T GetNew<T>();

        #region AddGUI options

        #region Int
        public static void IntAddGUI<TValue>(this SDictionary<int, TValue> dict, ref int toAdd) where TValue : new()
        {
            IntAddGUI(dict, ref toAdd, () => new TValue());
        }
        /// <summary>
        /// Extension method for String add GUI that initilizes new values with defualt
        /// </summary>
        public static void IntAddGUID<TValue>(this SDictionary<int, TValue> dict, ref int toAdd)
        {
            IntAddGUI(dict, ref toAdd, () => default);
        }

        public static void IntAddGUI<TValue>(this SDictionary<int, TValue> dict, ref int toAdd, GetNew<TValue> getNew)
        {
            EditorGUILayout.LabelField("New:", GUILayout.Width(45));
            toAdd = EditorGUILayout.IntField(toAdd);
            if (GUILayout.Button(new GUIContent("Add"), GUILayout.Width(45)))
            {
                if (!dict.ContainsKey(toAdd))
                {
                    dict.Add(toAdd, getNew());
                }
                toAdd = 0;
                GUIUtility.keyboardControl = 0;
            }
        }
        #endregion

        #region String
        public static void StringAddGUI<TValue>(this SDictionary<string, TValue> dict, ref string toAdd) where TValue : new()
        {
            StringAddGUI(dict, ref toAdd, () => new TValue());
        }
        /// <summary>
        /// Extension method for String add GUI that initilizes new values with defualt
        /// </summary>
        public static void StringAddGUID<TValue>(this SDictionary<string, TValue> dict, ref string toAdd)
        {
            StringAddGUI(dict, ref toAdd, () => default);
        }

        private static void StringAddGUI<TValue>(SDictionary<string, TValue> dict, ref string toAdd, GetNew<TValue> getNew)
        {
            EditorGUILayout.LabelField("New:", GUILayout.Width(45));
            toAdd = EditorGUILayout.TextField(toAdd);
            if (GUILayout.Button(new GUIContent("Add"), GUILayout.Width(45)))
            {
                if (!string.IsNullOrWhiteSpace(toAdd) && !dict.ContainsKey(toAdd))
                {
                    dict.Add(toAdd, getNew());
                }
                toAdd = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
        }
        #endregion

        #region Enum
        public static void EnumAddGUI<TKey, TValue>(this SDictionary<TKey, TValue> dict) where TKey : System.Enum where TValue : new()
        {
            EnumAddGUI(dict, () => new TValue());
        }

        public static void EnumAddGUIVal<TKey, TValue>(this SDictionary<TKey, TValue> dict) where TKey : System.Enum
        {
            EnumAddGUI(dict, () => default);
        }

        private static void EnumAddGUI<TKey, TValue>(this SDictionary<TKey, TValue> dict, GetNew<TValue> getNew) where TKey : System.Enum
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("+"), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var t in EnumUtils.GetValues<TKey>())
                    if (!dict.ContainsKey(t))
                        menu.AddItem(new GUIContent(t.ToString()), false, (obj) => dict.Add((TKey)obj, getNew()), t);
                menu.ShowAsContext();
            }
        }
        #endregion

        #region Object Picker
        public static void ObjPickerAddGUIVal<TKey, TValue>(this SDictionary<TKey, TValue> dict, string filter = "", bool allowSceneObjects = false) where TKey : UnityEngine.Object
        {
            dict.ObjPickerAddGUI(filter, allowSceneObjects, default);
        }

        public static void ObjPickerAddGUI<TKey, TValue>(this SDictionary<TKey, TValue> dict, string filter = "", bool allowSceneObjects = false) where TKey : UnityEngine.Object where TValue : new()
        {
            dict.ObjPickerAddGUI(filter, allowSceneObjects, () => new TValue());
        }

        private static void ObjPickerAddGUI<TKey, TValue>(this SDictionary<TKey, TValue> dict, string filter, bool allowSceneObjects, GetNew<TValue> getNew) where TKey : UnityEngine.Object
        {
            Event e = Event.current;
            if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
            {
                TKey item = EditorGUIUtility.GetObjectPickerObject() as TKey;
                if (item == null)
                    return;
                dict.Add(item, getNew());
                e.Use();
                return;
            }
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<TKey>(null, allowSceneObjects, filter, 1);
        }
        #endregion

        public static void GenericMenuAddGUI<TKey, TValue>(this SDictionary<TKey, TValue> dict, GenericMenu menu)
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("+"), FocusType.Keyboard))
            {
                menu.ShowAsContext();
            }
        }

        #endregion

        #region ValueGUI options

        public static TValue ValueGUIObjAllowSceneAssets<TKey, TValue>(this SDictionary<TKey, TValue> dict, TValue value) where TValue : UnityEngine.Object
        {
            return EditorUtils.ObjectField(value, true);
        }

        public static TValue ValueGUIObj<TKey, TValue>(this SDictionary<TKey, TValue> dict, TValue value) where TValue : UnityEngine.Object
        {
            return EditorUtils.ObjectField(value, false);
        }

        #endregion

        #region KeyGUI options

        public static void KeyGUIFixedWidth<TKey, TValue>(this SDictionary<TKey, TValue> dict, TKey key, float width)
        {
            EditorGUILayout.LabelField(key.ToString(), GUILayout.Width(width));
        }

        #endregion

        public static void DoGUILayout<TKey, TValue>(this SDictionary<TKey, TValue> dict, ValueGUI<TValue> valueGUI, AddGUI addGUI, string title, bool oneLine = false)
        {
            void kGUI(TKey key) => EditorGUILayout.LabelField(key.ToString(), GUILayout.MaxWidth(oneLine ? 100 : 250));
            dict.DoGUILayout(kGUI, valueGUI, addGUI, title, oneLine);
        }

        public static void DoGUILayout<TKey, TValue>(this SDictionary<TKey, TValue> dict, KeyGUI<TKey> kGUI, ValueGUI<TValue> valueGUI, AddGUI addGUI, string title, bool oneLine = false)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + ": " + dict.Count, EditorUtils.Bold, GUILayout.MaxWidth(120));
            GUILayout.Space(-20);
            //GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear"))
                dict.Clear();
            addGUI();
            GUILayout.EndHorizontal();
            if (dict.Count > 0)
                DoGUILayout(dict, kGUI, valueGUI, oneLine);
        }

        private static void DoGUILayout<TKey, TValue>(SDictionary<TKey, TValue> dict, KeyGUI<TKey> kGUI, ValueGUI<TValue> valueGUI, bool oneLine)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            TKey toDelete = default;
            bool delete = false;
            TKey[] keys = new TKey[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            System.Array.Sort(keys);
            foreach (var key in keys)
            {
                GUILayout.BeginHorizontal();
                kGUI(key);
                GUILayout.Space(1);
                if (oneLine && dict.ContainsKey(key))
                    dict[key] = valueGUI(dict[key]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(45)))
                {
                    toDelete = key;
                    delete = true;
                }
                GUILayout.EndHorizontal();
                if (!oneLine && dict.ContainsKey(key))
                    dict[key] = valueGUI(dict[key]);
            }
            if (delete)
                dict.Remove(toDelete);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}
