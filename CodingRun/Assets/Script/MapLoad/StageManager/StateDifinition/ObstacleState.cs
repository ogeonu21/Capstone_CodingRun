using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObstacleState : MonoBehaviour, IStageState
{
    [Range(1,10)]
    public float obstacleDuration = 3f;
    private float currentTime = 0f;
    private Coroutine coroutine = null;
    public StageManager manager = null;


    public ObstacleState(StageManager manager) {
        this.manager = manager;
    }

    public void Enter()
    {
        Debug.Log("Obstacle Stage 진입");
        coroutine = StartCoroutine(manager.SpawnItems());
    }

    public void UpdateState()
    {
        Debug.Log("now 장애물 stage!!");
        // StageManager의 SpawnObstacle을 SpawnTime마다 실행
    }

    public void Exit()
    {
        StopCoroutine(coroutine);
        // StageManager에서 ChangeState를 실행할때 정리하는 로직직
    }

    IEnumerator TimerCoroutine(float time) {
        currentTime = time;

        while (currentTime < 0) {
            yield return new WaitForSeconds(1);
            currentTime--;
        }
    }
}