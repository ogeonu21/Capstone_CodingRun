using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObstacleState : MonoBehaviour, IStageState
{
    private StageManager manager = null;
    //타이머 
    [Range(1f, 30f)]
    public float obstacleDuration = 10f;

    private void Awake() {
        manager = FindObjectOfType<StageManager>();
    }

    public void Enter()
    {
        Debug.Log("Obstacle Stage 진입");
        manager.StartSpawn();
        TimerExtension.StartTimer(obstacleDuration, () => {
            manager.ChangeState(StageState.QUESTION_STATE);
        });
        TimerExtension.StartTimer(obstacleDuration-manager.adjustTime, () => {
            manager.StopSpawn();
        }); 
    }

    public void UpdateState()
    {
        // StageManager의 SpawnObstacle을 SpawnTime마다 실행
    }

    public void Exit()
    {
        // StageManager에서 ChangeState를 실행할때 정리하는 로직.
        
    }
}