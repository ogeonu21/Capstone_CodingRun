using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;

public class Timer : MonoBehaviour
{
    [SerializeField] float timeToCompleteQuestion = 15;

    public bool isAnsweringQuestion;
    public bool loadGameScene;
    public bool timeUp;
    public float fillFraction; //타이머 진행률
    float timerValue;

    void Start()
    {
        timerValue = timeToCompleteQuestion;
    }

    void Update()
    {
        UpdateTimer();
    }

    public void CancelTimer(){
        timerValue = 0;
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
            }
        }

    }
}
