using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
    /// <summary>
    /// Serializable multiset wrapper.
    /// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
    /// </summary>
    /// <typeparam name="T"> The type contained in the collection </typeparam>
    public class SMultiSet<T> : ISerializationCallbackReceiver, IEnumerable<T>
    {
        private Dictionary<T, int> _dictionary = new Dictionary<T, int>(); // Internal dictionary interface
        [SerializeField] private List<T> _keys = new List<T>(); // Serlializable list of keys; Keys and values match up 1-to-1
        [SerializeField] private List<int> _frequencies = new List<int>(); // Serlializable list of values

        #region Dictionary Implementation
        //The number of distict items (not including duplicate) in the multiset 
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }
        //The number of items in the multiset including duplicates: O(n)
        public int TotalCount
        {
            get
            {
                int count = 0;
                foreach (int freq in _dictionary.Values)
                    count += freq;
                return count;
            }
        }
        //Clear the whole multiset
        public void Clear()
        {
            _dictionary.Clear();
        }
        //Removes the item from the multiset regardless of its frequency
        public void ClearItem(T item)
        {
            _dictionary.Remove(item);
        }
        //Return if there is at least one of the item in the set
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }
        //Return the frequency of the item in the multiset (read-only)
        public int Freq(T item)
        {
            return Contains(item) ? _dictionary[item] : 0;
        }
        //Adds the item to the multiset if not already an element, else increments its frequency by amount
        public void Add(T item, int amount = 1)
        {
            if (_dictionary.ContainsKey(item))
                _dictionary[item] += amount;
            else
                _dictionary.Add(item, amount);
        }
        //Removes the item from the multiset if its frequency is less than amount, else lowers the frequency by amount
        public void Remove(T item, int amount = 1)
        {
            if (_dictionary[item] <= amount)
                _dictionary.Remove(item);
            else
                _dictionary[item] -= amount;
        }
        #endregion

        #region IEnumarable Implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }
        #endregion

        #region Serialization
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _frequencies.Clear();
            foreach (var kvp in _dictionary)
            {
                _keys.Add(kvp.Key);
                _frequencies.Add(kvp.Value);
            }
        }

        // Convert lists back into dictionary
        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<T, int>();
            for (int i = 0; i != System.Math.Min(_keys.Count, _frequencies.Count); ++i)
            {
                try
                {
                    _dictionary.Add(_keys[i], _frequencies[i]);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("SMultiSet Deserialization Error: " + e.ToString() + " " + this.ToString());
                }
            }
        }
        #endregion
    } 
}
