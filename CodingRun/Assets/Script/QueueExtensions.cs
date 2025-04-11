using System.Collections.Generic;
using UnityEngine;

public static class QueueExtensions
{
    public static void Shuffle<T>(this Queue<T> queue)
    {
        List<T> list = new(queue);
        int n = list.Count;

        // Fisher–Yates shuffle
        for (int i = 0; i < n; i++)
        {
            int rand = Random.Range(i, n);
            (list[i], list[rand]) = (list[rand], list[i]);
        }

        queue.Clear();
        foreach (T item in list)
        {
            queue.Enqueue(item);
        }
    }

    public static void Print<T>(this Queue<T> queue) {
        foreach(var item in queue) {
            Debug.Log(item);
        }
    }
}

