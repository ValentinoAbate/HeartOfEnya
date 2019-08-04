using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SerializableCollections
{
    /// <summary> 
    /// Serializable dictionary wrapper (native Dictionary serialization not yet available).
    /// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
    /// </summary>
    public class SDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>(); // Internal dictionary interface
        [SerializeField] private List<TKey> _keys = new List<TKey>(); // Serlializable list of keys; Keys and values match up 1-to-1
        [SerializeField] private List<TValue> _values = new List<TValue>(); // Serlializable list of values

        #region Dictionary Implementation
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }
        public void Clear()
        {
            _dictionary.Clear();
        }
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }
        public void Remove(TKey key)
        {
            _dictionary.Remove(key);
        }
        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
            }
        }
        public Dictionary<TKey, TValue>.KeyCollection Keys { get { return _dictionary.Keys; } }
        #endregion

        #region IEnumarable Implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        #endregion

        #region Serialization
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (var kvp in _dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        // Convert lists back into dictionary
        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            for (int i = 0; i != System.Math.Min(_keys.Count, _values.Count); ++i)
            {
                try
                {
                    _dictionary.Add(_keys[i], _values[i]);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("SDict Deserialization Error: " + e.ToString() + " " + this.ToString());
                }
            }
        }
        #endregion
    }
}
