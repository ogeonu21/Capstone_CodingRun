using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    //Object 
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PoolObject"))
        {
            if (other.CompareTag("Coin")) 
            ObjectPoolManager.Instance.ReturnObject(ObjectType.COIN, other.gameObject.GetComponent<UnifiedItem>());
            else if (other.CompareTag("Wall"))
            ObjectPoolManager.Instance.ReturnObject(ObjectType.WALL, other.gameObject.GetComponent<UnifiedObstacle>());
            else if (other.CompareTag("Hall"))
            ObjectPoolManager.Instance.ReturnObject(ObjectType.HALL, other.gameObject.GetComponent<UnifiedObstacle>());
            else
            Debug.LogError("unknown poolObject detected! Check its tag!!");
        }
    }
}
