using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [Header("Instant Amounts")]
    [Space(10)]
    [Range(0, 100)] public int coinAmount = 30;
    [Range(0, 20)] public int obstacleAmount = 10;
    [Range(0, 1f)] public float wallRatio = 0.7f;
    [Range(0, 1f)] public float hallRatio = 0.3f;

    [Header("Prefabs")]
    [Space(10)]
    public GameObject coinPref = null;
    public GameObject wallPref = null;
    public GameObject hallPref = null;

    private Queue<GameObject> coinPool = new();
    private Queue<GameObject> obstaclePool = new();

    private void InitCoinPool() {
        if (coinPref == null) { Debug.LogError("coinPref is null"); return; }
        for (int i = 0; i < coinAmount; i++) {
            GameObject obj = Instantiate(coinPref);
            coinPool.Enqueue(obj);
            obj.SetActive(false);
        }
        Debug.Log("Initialized CoinPool Successfully");
    }

    //TODO : Obstacle 오브젝트들을 ObstaclePool안에 넣는 함수 만들기 (생성비율에 따라 만들것)
    private void InitObstaclePool() {
        
    }

    private GameObject GetCoin() {
        if (coinPool.Count > 0) { //coinPool에 오브젝트가 남아있으면 풀에서 꺼냄
            GameObject obj = coinPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(coinPref); //안남아있으면 새로 만들어서 반환
    }

    //TODO : 풀에서 Obstacle Get하는 메서드 구현 (구멍인지 벽인지는 상관x 확률상 생성이기 때문에)
    private GameObject GetObstacle() {
        return null;
    }

    private void ReturnCoin(GameObject obj) {
        obj.SetActive(false);
        coinPool.Enqueue(obj);
    }

    private void ReturnObstacle(GameObject obj) {
        obj.SetActive(false);
        obstaclePool.Enqueue(obj);
    }
}
