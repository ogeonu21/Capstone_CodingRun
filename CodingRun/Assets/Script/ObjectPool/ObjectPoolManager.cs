using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [Header("Instant Amounts")]
    [Range(0, 100)] public int coinAmount = 30;

    [Header("Prefabs")]
    public List<PoolData> poolDataList;

    private Dictionary<ObjectType, IObjectPool> pools = new();

    public static ObjectPoolManager Instance;

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
        }
        else if(Instance != this){
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitPoolsFromList();

        //테스트 예제 (obj를 받을때 MonoBehaviour로 받음음)
        //MonoBehaviour obj = GetObject(ObjectType.COIN);
        
    }

    private void InitPoolsFromList() {
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
            var pool = new ObjectPool<MonoBehaviour>(comp);
            pool.InitObjectPool(data.amount, comp);

            if (!pools.ContainsKey(type))
            {
                pools.Add(type, pool);
            }
        }
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
