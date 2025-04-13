using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float laneDistance = 5.0f;       // 레인 간격
    private int currentLane = 1;            // 현재 레인 (0: 왼쪽, 1: 중앙, 2: 오른쪽)

    void Update()
    {
        HandleInput();   // 입력에 따른 레인 변화
        MoveToLane();    // 위치 이동
    }

    void HandleInput()   // 입력 처리
    {
        var input = InputManager.Instance.Swipe;        // InputManager에서 스와이프 방향 가져오기

        if (input == InputManager.SwipeDirection.Left && currentLane > 0)           //왼쪽으로 스와이프한 경우 왼쪽으로 한칸이동(최소 0)
            currentLane--;
        else if (input == InputManager.SwipeDirection.Right && currentLane < 2)      //오른쪽으로 스와이프한 경우 오른쪽으로 한칸이동(최대 2)
            currentLane++;
    }

    void MoveToLane()                                   // 현재 레인에 맞게 플레이어 위치 이동
    {
        Vector3 targetPos = transform.position;         // 현재 위치를 타겟 위치로 설정
        targetPos.x = (currentLane - 1) * laneDistance; // 현재 레인에 맞게 x좌표 설정
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);// 현재 위치에서 타겟 위치로 부드럽게 이동
    }
}