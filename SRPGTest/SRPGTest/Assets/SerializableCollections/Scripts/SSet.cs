using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SerializableCollections
{
    /// <summary>
    /// Serializable HashSet wrapper (native HashSet serialization not yet available).
    /// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
    /// </summary> 
    [System.Serializable]
    public class SSet<T> : ISerializationCallbackReceiver, IEnumerable<T>
    {
        HashSet<T> _hashset = new HashSet<T>(); // Internal HashSet interface
        [SerializeField] private List<T> _items = new List<T>(); // Serializable list of items

        #region Set Implementation
        public int Count { get { return _hashset.Count; } }
        public bool Contains(T item)
        {
            return _hashset.Contains(item);
        }
        public void Add(T item)
        {
            if (!_hashset.Contains(item))
                _hashset.Add(item);
        }
        public void Remove(T item)
        {
            _hashset.Remove(item);
        }
        public void Clear()
        {
            _hashset.Clear();
        }
        public T[] Items
        {
            get
            {
                T[] ret = new T[Count];
                _hashset.CopyTo(ret);
                return ret;
            }
        }
        #endregion

        #region IEnumerable Implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hashset.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _hashset.GetEnumerator();
        }
        #endregion

        #region Serialization
        public void OnBeforeSerialize()
        {
            _items.Clear();
            foreach (T item in _hashset)
                _items.Add(item);
        }

        // Convert list back into set
        public void OnAfterDeserialize()
        {
            _hashset = new HashSet<T>();
            foreach (T item in _items)
            {
                try
                {
                    _hashset.Add(item);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("SSet Deserialization Error: " + e.ToString() + " " + this.ToString());
                }

            }
        }
        #endregion
    } 
}

