using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    [Header("회전 속도 설정(float)")]
    [Tooltip("3D 이미지를 회전시키는 속도입니다.")]
    public float rotationSpeed = 30f;

    [Header("코인 점수 (Score)")]
    [Tooltip("코인을 획득했을 때 증가하는 점수")]
    public float coinScore = 100f;
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

    // 플레이어와 충돌 시 점수 증가 로직 추가
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            GameManager.Instance.Score += coinScore;
            // 새로운 최고 점수 저장 (필요에 따라 호출)
            GameManager.Instance.SaveHighScore();
            // 코인 비활성화
            DisableObject();
        }
    }
}
