using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool
{
    MonoBehaviour GetObject();
    void ReturnObject(MonoBehaviour obj);
    void InitObjectPool(int amount, MonoBehaviour prefab);
}
