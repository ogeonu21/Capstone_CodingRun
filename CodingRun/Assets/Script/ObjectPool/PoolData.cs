using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolData
{
    public ObjectType type;
    public GameObject prefab;
    public int amount;
}
