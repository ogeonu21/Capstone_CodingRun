using System;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [Header("Instant Amounts")]
    [Range(0, 100)] public int coinAmount = 30;

    [Header("Prefabs")]
    public List<PoolData> poolDataList;

    private Dictionary<ObjectType, IObjectPool> pools = new();

    private ObjectPool<MonoBehaviour> pool;

    void Start()
    {
        //테스트 예제 (obj를 받을때 MonoBehaviour로 받음음)
        //MonoBehaviour obj = GetObject(ObjectType.COIN);

    }


    public void InitPoolsFromList()
    {
        foreach (PoolData data in poolDataList)
        {
            if (!data.prefab.TryGetComponent(out MonoBehaviour component))
            {
                Debug.LogWarning($"Prefab {data.prefab.name} does not have a MonoBehaviour component.");
                continue;
            }

            ObjectType type = data.type;
            MonoBehaviour comp = data.prefab.GetComponent<MonoBehaviour>();

            // ObjectPool<MonoBehaviour>로 처리
            pool = new ObjectPool<MonoBehaviour>(comp);
            pool.InitObjectPool(data.amount, comp);

            if (!pools.ContainsKey(type))
            {
                pools.Add(type, pool);
            }
        }
    }

    public void ClearPool()
    {
        if (pool == null) return;
        Debug.Log("ClearPool 실행됨 !!!!!!!!!!!!!!!!!!!! Count : "+pool.objectPool.Count);
        while (pool.objectPool.Count > 0) pool.objectPool.Dequeue();
        Debug.Log("ClearPool 다 실행됨!!!!!!!!!!!!!!!!!! Count : "+pool.objectPool.Count);
    }

    public MonoBehaviour GetObject(ObjectType type)
    {
        pools.TryGetValue(type, out IObjectPool value);

        if (value == null)
        {
            Debug.LogError("ObjectPool is null!");
            return null;
        }

        return value.GetObject();
    }

    public void ReturnObject(ObjectType type, MonoBehaviour obj)
    {
        pools.TryGetValue(type, out IObjectPool value);
        if (value == null) return;
        value.ReturnObject(obj);
    }
    
}
