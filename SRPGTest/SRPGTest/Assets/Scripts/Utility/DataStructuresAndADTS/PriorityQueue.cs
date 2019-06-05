using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A c# priority queue implementation for pathfinding
/// Currently based on inefficient backend for testing purposes
/// Will implement using a binary heap later
/// </summary>
public class PriorityQueue<T>
{
    private BinaryMinHeap<Item> items;

    public bool Empty { get => items.Empty; }

    public PriorityQueue()
    {
        items = new BinaryMinHeap<Item>();
    }
    public PriorityQueue(int capacity)
    {
        items = new BinaryMinHeap<Item>(capacity);
    }

    public T Peek()
    {
        if (Empty)
            return default(T);
        return items.PeekMin().item;
    }

    public T Dequeue()
    {
        return items.ExtractMin().item;
    }

    public void Enqueue(T item, float priority)
    {
        items.Insert(new Item() { item = item, priority = priority });
    }

    private struct Item : IComparable<Item>
    {
        public float priority;
        public T item;

        public int CompareTo(Item other)
        {
            return priority.CompareTo(other.priority);
        }
    }

}
