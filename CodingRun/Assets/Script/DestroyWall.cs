using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    //Object 레이어와 충돌시, 충돌한 물체를 삭제
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            Destroy(collision.gameObject);
        }
    }
}
