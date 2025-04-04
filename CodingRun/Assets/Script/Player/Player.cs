using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float laneDistance = 5.0f; // 좌중우 레인 간 간격 & 이동거리 5씩이동
    private int currentLane = 1;      // 0 = 왼쪽, 1 = 중앙, 2 = 오른쪽

    private Vector2 touchStart; //터치 시작 지점
    private bool isSwiping = false; //스와핑 여부

    void Update()
    {
        HandleSwipeOrKey();
        MoveToLane();
    }

    void MoveToLane()
    {
        Vector3 targetPos = transform.position;
        targetPos.x = (currentLane - 1) * laneDistance; //플레이어 x값 왼쪽(-1)=-1*5   가운데(1)=0*5  오른쪽(1)=1*5
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f); //부드러운 움직임,10f조정시 레인->레인이동시간 조절 가능(크면 클수록 빠름)
    }

    void HandleSwipeOrKey()
    {
        // 모바일 스와이프
        if (Input.touchCount == 1)     //손가락 1개만 터치중일떄
        {
            Touch touch = Input.GetTouch(0);    //첫 번째 터치 정보 호출

            if (touch.phase == TouchPhase.Began)    //터치했을때
            {
                touchStart = touch.position;    //터치 시작 x값 저장
                isSwiping = true;               //스와이핑 상태로 전환
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)  //스와이핑중 & 스와이핑 종료시
            {
                float deltaX = touch.position.x - touchStart.x; 터치 시작 x값과 터치가 끝난 x값 차이 계산

                if (Mathf.Abs(deltaX) > 50f)    //x값의 차이가 50f을 넘기면 스와이핑으로 인정
                {
                    if (deltaX > 0 && currentLane < 2) currentLane++;   //오른쪽 이동
                    else if (deltaX < 0 && currentLane > 0) currentLane--;  //왼쪽 이동 (레인범위를 초과하지 않도록)
                }

                isSwiping = false;  //한번의 스와이핑 끝났으니 상태 초기화
            }
        }

        // PC 키보드 테스트용 입력
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0) //왼쪽 화살표
        {
            currentLane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)   //오른쪽 화살표
        {
            currentLane++;
        }
    }
}
