using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    [Header("회전 속도 설정(float)")]
    [Tooltip("3D 이미지를 회전시키는 속도입니다.")]
    public float rotationSpeed = 30f;
    private void Start()
    {
        
    }
    protected override void Update() //Item 스크립트의 Update에 RotateObject를 추가한 Update메소드
    {
        base.Update();
        RotateObject();
    }
    private void RotateObject()
    {
        // Y축을 기준으로 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
