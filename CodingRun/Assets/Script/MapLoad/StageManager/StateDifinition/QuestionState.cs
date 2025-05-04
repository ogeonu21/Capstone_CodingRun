using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class QuestionState : MonoBehaviour, IStageState
{
    private Timer timer = null;
    private StageManager manager = null;

    private void Awake() {
        timer = FindObjectOfType<Timer>();
        manager = FindAnyObjectByType<StageManager>();
    }

    public void Enter()
    {
        Debug.Log("문제 Stage Entered!!");
        StartQuiz();
    }

    public void UpdateState()
    {
    }

    public void Exit()
    {
        Debug.Log("문제 Stage Finished!!");
        //State 전환될때 처리 로직
        FindAnyObjectByType<Quiz>().SetQuestionPanelActive(false);
    }
    private void StartQuiz() {
        FindAnyObjectByType<GameManager>().HandleQuizTransition();
    }
}