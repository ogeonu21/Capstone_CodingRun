using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
   public static InputManager Instance;     //싱글톤

    public enum SwipeDirection { None, Left, Right } //스와이프 결과 구분(없음, 왼쪽, 오른쪽)
    public SwipeDirection Swipe { get; private set; }//스와이프 결과 저장

    private Vector2 touchStart;      //터치 시작 위치
    private bool isSwiping;          //스와이프 중인지 여부

    void Awake()                     //싱글톤 최기화
    {
        Instance = this;
    }

    void Update()
    {
        Swipe = SwipeDirection.None;    //스와이프 초기화

#if UNITY_EDITOR        //키보드
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Swipe = SwipeDirection.Left;       
        else if (Input.GetKeyDown(KeyCode.RightArrow)) Swipe = SwipeDirection.Right;
#else                   //모바일 스와이프 
        if (Input.touchCount == 1)      //손가락 하나로 터치일때
        {
            Touch touch = Input.GetTouch(0);        //터치 입력 가져오기

            if (touch.phase == TouchPhase.Began)    //터치 시작
            {
                touchStart = touch.position;        //터치 시작 위치 저장
                isSwiping = true;                   //스와이프 시작
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)  //터치 끝 & 스와이프 중중
            {
                float deltaX = touch.position.x - touchStart.x;     //시작과 끝 X값 위치 차이 계산

                if (Mathf.Abs(deltaX) > 50f)                        //이동 거리가 50픽셀 이상일때
                    Swipe = deltaX > 0 ? SwipeDirection.Right : SwipeDirection.Left;    //스와이프 방향 결정

                isSwiping = false;                                  //스와이프 종료
            }
        }
#endif
    }
}