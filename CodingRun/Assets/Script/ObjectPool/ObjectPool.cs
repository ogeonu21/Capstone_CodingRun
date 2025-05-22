using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool where T : MonoBehaviour
{
    public readonly Queue<T> objectPool = new Queue<T>();
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
    while (objectPool.Count > 0)
    {
        T obj = objectPool.Dequeue();
        if (obj != null && !obj.Equals(null))
        {
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Destroy된 오브젝트가 큐에 있었음. 제거됨.");
    }

    T newObj = Object.Instantiate(prefab);
    return newObj;
}


    public void ReturnObject(MonoBehaviour obj)
    {
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj as T);
    }
}
