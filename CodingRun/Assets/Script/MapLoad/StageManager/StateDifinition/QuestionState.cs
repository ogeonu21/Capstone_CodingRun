using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class QuestionState : MonoBehaviour, IStageState
{
    public StageManager manager = null;
    private Coroutine coroutine = null;

    public void Enter()
    {
        Debug.Log("문제 Stage Entered!!");
        coroutine = StartCoroutine(manager.ShowQuiz());
    }

    public void UpdateState()
    {
        Debug.Log("now 문제 Stage!!!!!!");
        //문제가 풀렸는지 확인    이벤트로 처리할 수 있는지 검토
    }

    public void Exit()
    {
        Debug.Log("문제 Stage Finished!!");
        StopCoroutine(coroutine);
    }
}