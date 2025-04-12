using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool where T : MonoBehaviour
{
    private readonly Queue<T> objectPool = new Queue<T>();
    private readonly T prefab;

    public ObjectPool(T prefab)
    {
        this.prefab = prefab;
    }

    public void InitObjectPool(int amount, MonoBehaviour prefab)
    {
        for (int i = 0; i < amount; i++)
        {
            T obj = Object.Instantiate(prefab) as T;
            objectPool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }

    public MonoBehaviour GetObject()
    {
        if (objectPool.Count > 0)
        {
            T obj = objectPool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        return Object.Instantiate(prefab);
    }

    public void ReturnObject(MonoBehaviour obj)
    {
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj as T);
    }
}
