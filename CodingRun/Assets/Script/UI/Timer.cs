using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] float defaultTimeToCompleteQuestion = 15f;    // 기본 문제 풀이 시간
    float timeToCompleteQuestion;    // 현재 문제의 풀이 시간

    public bool isAnsweringQuestion;
    public bool loadGameScene;
    public bool timeUp;
    public float fillFraction; //타이머 진행률
    public float timerValue;  // public으로 변경
    
    // 타이머 이벤트 추가
    public UnityEvent onTimeUp = new UnityEvent();

    void Start()
    {
        timeToCompleteQuestion = defaultTimeToCompleteQuestion;
        ResetTimer();
    }

    void Update()
    {
        UpdateTimer();
    }

    public void CancelTimer(){
        timerValue = 0;
    }

    // 타이머를 초기화합니다.
    public void ResetTimer()
    {
        timerValue = timeToCompleteQuestion;
        isAnsweringQuestion = true;
        timeUp = false;
    }

    // 문제별 시간 설정
    public void SetQuestionTime(float time)
    {
        timeToCompleteQuestion = time;
        ResetTimer();
    }

    void UpdateTimer(){
        timerValue -= Time.deltaTime;
        if(isAnsweringQuestion) //질문에 답변 중인 경우
        {
            if(timerValue > 0)
            {
                fillFraction = timerValue / timeToCompleteQuestion;
            }
            else
            {
                isAnsweringQuestion = false;
                timeUp = true;
                
                // 타이머 종료 시 이벤트 발생
                onTimeUp.Invoke();
                Debug.Log("타이머 종료 이벤트 발생");
            }
        }
    }
}
