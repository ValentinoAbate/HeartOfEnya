using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serializable dictionary wrapper (native Dictionary serialization not yet available)
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
public class SerializableMultiDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>(); // Internal dictionary interface
    private Dictionary<TKey, int> _frequencyDict = new Dictionary<TKey, int>();
    [SerializeField] private List<TKey> _keys = new List<TKey>(); // Serlializable list of keys; Keys and values match up 1-to-1
    [SerializeField] private List<TValue> _values = new List<TValue>(); // Serlializable list of values
    [SerializeField] private List<int> _frequencies = new List<int>();

    #region Dictionary Implementation
    //Total number of unique keys in the dictionary
    public int Count
    {
        get
        {
            return _dictionary.Count;
        }
    }
    //Total number of keys (including frquencies)
    public int TotalCount
    {
        get
        {
            int count = 0;
            foreach (int freq in _frequencyDict.Values)
                count += freq;
            return count;
        }
    }
    //Clear the whole multidict
    public void Clear()
    {
        _dictionary.Clear();
        _frequencyDict.Clear();
    }
    //Removes the item from the multiset regardless of its frequency
    public void ClearItem(TKey item)
    {
        _dictionary.Remove(item);
        _frequencyDict.Remove(item);
    }
    //Return if there is at least one of the item in the set
    public bool ContainsKey(TKey item)
    {
        return _dictionary.ContainsKey(item);
    }
    //Return the frequency of the item in the multidict (read-only)
    public int Frequency(TKey item)
    {
        return _frequencyDict[item];
    }
    //Adds the item to the multiset if not already an element, else increments its frequency by amount
    public void Add(TKey key, TValue value, int amount = 1)
    {
        if (_dictionary.ContainsKey(key))
            _frequencyDict[key] += amount;
        else
        {
            _dictionary.Add(key, value);
            _frequencyDict.Add(key, amount);
        }
    }
    //Removes the item from the multiset if its frequency is less than amount, else lowers the frequency by amount
    public void Remove(TKey key, int amount = 1)
    {
        if (_frequencyDict[key] <= amount)
        {
            _dictionary.Remove(key);
            _frequencyDict.Remove(key);
        }
        else
            _frequencyDict[key] -= amount;
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
        _frequencies.Clear();
        _frequencies.AddRange(_frequencyDict.Values);
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
                _frequencyDict.Add(_keys[i], _frequencies[i]);
            }
            catch (System.ArgumentException)
            {
                continue;
            }
        }
    }
    #endregion
}
