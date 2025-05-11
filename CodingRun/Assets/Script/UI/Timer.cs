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

    private bool isPaused = false;  // 일시정지 상태를 나타내는 변수 추가
    private bool isQuizTime = false; // 퀴즈 시간인지 여부를 나타내는 변수 추가

    void Start()
    {
        timeToCompleteQuestion = defaultTimeToCompleteQuestion;
        ResetTimer();
    }

    void Update()
    {
        UpdateTimer();
        CheckTimeScale();  // Time.timeScale 체크
    }

    public void CancelTimer(){
        timerValue = 0;
        isAnsweringQuestion = false;
        isQuizTime = false;
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
        isAnsweringQuestion = true;
        isQuizTime = true;
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

    // Time.timeScale 체크
    private void CheckTimeScale()
    {
        if (Time.timeScale == 0f && !isPaused)
        {
            // 게임이 일시정지된 경우
            isPaused = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetQuizPanelByTimerState(false);
            }
            Debug.Log("게임 일시정지로 인한 타이머 일시정지");
        }
        else if (Time.timeScale == 1f && isPaused)
        {
            // 게임이 재개된 경우
            isPaused = false;
            if (GameManager.Instance != null && isQuizTime)
            {
                GameManager.Instance.SetQuizPanelByTimerState(true);
            }
            Debug.Log("게임 재개로 인한 타이머 재개");
        }
    }

    void UpdateTimer()
    {
        if (isAnsweringQuestion && !isPaused)  // 일시정지 상태가 아닐 때만 타이머 감소
        {
            timerValue -= Time.deltaTime;
            if (timerValue > 0)
            {
                fillFraction = timerValue / timeToCompleteQuestion;
            }
            else
            {
                isAnsweringQuestion = false;
                isQuizTime = false;
                timeUp = true;
                
                // 타이머 종료 시 이벤트 발생
                onTimeUp.Invoke();
                Debug.Log("타이머 종료 이벤트 발생");
            }
        }
    }
}
