using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody rb;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다.");
            enabled = false;
        }
    }
    protected virtual void Update()
    {
        CheckOutOfBounds();
    }

    protected void DisableObject()
    {
        // ObjectPoolManager 없이 단순히 비활성화 (ObjectPoolManager구현시 적용 예정)
        gameObject.SetActive(false);
    }

    private void CheckOutOfBounds()
    {
        // 현재 카메라의 뷰포트 경계를 기준으로 오브젝트가 화면 밖에 나갔는지 확인
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.y < 0)
        {
            DisableObject();
        }
    }

    protected virtual void OnDisable()
    {
        // 비활성화될 때 필요한 작업 수행
        rb.velocity = Vector3.zero; // 속도 초기화
    }
}
