using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestionState : MonoBehaviour, IStageState
{
    StageManager manager = null;

    public QuestionState(StageManager manager) {
        this.manager = manager;
    }

    public void Enter()
    {
        Debug.Log("문제 Stage Entered!!");
        //문제 생성 로직
    }

    public void Update()
    {
        Debug.Log("now 문제 Stage!!!!!!");
        //문제가 풀렸는지 확인    이벤트로 처리할 수 있는지 검토
    }

    public void Exit()
    {
        Debug.Log("문제 Stage Finished!!");
        //State 전환될때 처리 로직직
    }
}