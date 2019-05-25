using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Serializable HashSet wrapper (native HashSet serialization not yet available)
// Should only be set in Editor more; Is read only in game mode
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
[System.Serializable]
public class SerializableSet<T> : ISerializationCallbackReceiver, IEnumerable<T>
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

    #region serialization
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
            _hashset.Add(item);
        }
    }
    #endregion
}

