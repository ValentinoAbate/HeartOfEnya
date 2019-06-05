using System.Collections;
using System.Collections.Generic;
using System;

public class BinaryMinHeap<T> where T : IComparable<T>
{
    public bool Empty { get => items.Count == 0; }
    private List<T> items;
    public BinaryMinHeap()
    {
        items = new List<T>();
    }
    public BinaryMinHeap(int capacity)
    {
        items = new List<T>(capacity);
    }
    public BinaryMinHeap(IEnumerable<T> initialContent)
    {
        items = new List<T>(initialContent);
        if(items.Count > 1)
            Heapify(0);
    }

    public T PeekMin()
    {
        return items[0];
    }

    public T ExtractMin()
    {
        if (items.Count == 0)
            throw new IndexOutOfRangeException("Trying to extract from an empty heap");
        if (items.Count == 1)
        {
            T ret2 = items[0];
            items.Clear();
            return ret2;
        }

        T ret = items[0];
        Delete(ret);
        return ret;
    }

    public void DecreaseKey(T oldValue, T newValue)
    {
        int index = items.FindIndex((item) => item.CompareTo(oldValue) == 0);
        items[index] = newValue;
        // Make sure the Min Heap Property is not violated
        while (index != 0 && items[Parent(index)].CompareTo(items[index]) > 0)
        {
            Swap(index, Parent(index));
            index = Parent(index);
        }
    }

    public void Insert(T item)
    {
        items.Add(item);
        int index = items.Count - 1;
        // Fix the min heap property if it is violated 
        while (index != 0 && items[Parent(index)].CompareTo(items[index]) > 0)
        {
            Swap(index, Parent(index));
            index = Parent(index);
        }
    }

    public void Delete(T item)
    {
        int i = items.IndexOf(item);
        int last = items.Count - 1;
        items[i] = items[last];
        items.RemoveAt(last);
        Heapify(0);
    }

    private void Heapify(int index)
    {
        int l = LeftChild(index);
        int r = RightChild(index);
        int min = index;
        if (l < items.Count && items[l].CompareTo(items[index]) < 0)
            min = l;
        if (r < items.Count && items[r].CompareTo(items[min]) < 0)
            min = r;
        if (min != index)
        {
            Swap(index, min);
            Heapify(min);
        }
    }

    private int Parent(int i) => (i - 1) / 2;

    private int LeftChild(int i) => (2 * i + 1);

    private int RightChild(int i) => (2 * i + 2);

    private void Swap(int index1, int index2)
    {
        T temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
    }
}
