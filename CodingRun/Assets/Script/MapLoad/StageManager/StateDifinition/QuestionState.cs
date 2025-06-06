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

    private void Start() {
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
        FindAnyObjectByType<Quiz>().SetQuestionPanelActive(false);
        manager.cycleNum++;
        if (manager.cycleNum % manager.heartPerCycle == 0) manager.SpawnHeart();
    }
    private void StartQuiz() {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager 인스턴스가 없습니다!");
            return;
        }
        GameManager.Instance.HandleQuizTransition();
    }
}