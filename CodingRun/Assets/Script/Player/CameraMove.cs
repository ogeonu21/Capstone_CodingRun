using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Tooltip("따라갈 대상 (플레이어)")]
    public Transform target;

    [Tooltip("카메라 이동 속도 (Lerp 보간 속도)")]
    public float followSpeed = 5f;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollowX: 타겟이 지정되지 않았습니다!");
            return;
        }

        // 현재 카메라 위치의 Y, Z 저장
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 타겟의 x 위치만 따라가고, y, z는 고정
        Vector3 targetPosition = new Vector3(target.position.x, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }
}
