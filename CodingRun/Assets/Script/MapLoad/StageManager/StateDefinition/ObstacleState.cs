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
    


    public void Enter()
    {
        Debug.Log("Obstacle Stage 진입");
        // coroutine = StartCoroutine(TimerCoroutine(obstacleDuration));
    }

    public void Update()
    {
        // StageManager의 SpawnObstacle을 SpawnTime마다 실행
    }

    public void Exit()
    {
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
