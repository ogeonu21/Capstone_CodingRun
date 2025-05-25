using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] float defaultTimeToCompleteQuestion = 15f;    // 기본 문제 풀이 시간
    public float timeToCompleteQuestion;    // 현재 문제의 풀이 시간

    public bool isAnsweringQuestion;
    public bool loadGameScene;
    public bool timeUp;
    public float fillFraction; //타이머 진행률
    public float timerValue;  // public으로 변경
    
    // 타이머 이벤트 추가
    public UnityEvent onTimeUp = new UnityEvent();

    void Update()
    {
        // 게임 오버 상태일 때는 타이머 업데이트 하지 않음
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        UpdateTimer();
    }

    public void CancelTimer(){
        timerValue = 0;
        isAnsweringQuestion = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetQuizPanelByTimerState(false);
        }
    }

    // 타이머를 초기화합니다.
    public void ResetTimer()
    {
        timerValue = timeToCompleteQuestion;  // 타이머 값을 초기화
        timeUp = false;
    }

    // 타이머를 시작합니다.
    public void StartTimer()
    {
        timeToCompleteQuestion = defaultTimeToCompleteQuestion;
        ResetTimer();

        isAnsweringQuestion = true;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetQuizPanelByTimerState(true);
        }
    }

    // 문제별 시간 설정
    public void SetQuestionTime(float time)
    {
        timeToCompleteQuestion = time;
        ResetTimer();
    }

    void UpdateTimer()
    {
        if (isAnsweringQuestion)  // 일시정지 상태가 아닐 때만 타이머 감소
        {
            timerValue -= Time.deltaTime;
            if (timerValue > 0)
            {
                fillFraction = timerValue / timeToCompleteQuestion;
            }
            else
            {
                isAnsweringQuestion = false;
                timeUp = true;
                
                // 타이머 종료 시 이벤트 발생
                onTimeUp.Invoke();
            }
        }
    }
}
