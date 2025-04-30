using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    //Object 
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            ObjectPoolManager.Instance.ReturnObject(other.gameObject.GetComponent<UnifiedItem>().GetPoolType(), other.gameObject.GetComponent<UnifiedItem>());
        }
    }
}
