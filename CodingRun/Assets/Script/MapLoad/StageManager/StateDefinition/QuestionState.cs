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
        //문제 생성 로직
    }

    public void Update()
    {
        //문제가 풀렸는지 확인    이벤트로 처리할 수 있는지 검토
    }

    public void Exit()
    {
        //State 전환될때 처리 로직직
    }
}
