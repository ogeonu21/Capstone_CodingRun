using System;
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

    [Header("Instant Ratio")]
    [Range(0, 1f)] public float wallRatio = 0.7f;
    [Range(0, 1f)] public float hallRatio = 0.3f;

    [Header("Prefabs")]
    [Space(10)]
    public GameObject coinPref = null;
    public GameObject wallPref = null;
    public GameObject hallPref = null;

    private Queue<GameObject> coinPool = new();
    private Queue<GameObject> obstaclePool = new();

    void Start()
    {
        hallRatio = 1.0f - wallRatio; // 확률이 1 넘어가지 않도록 
        InitCoinPool();
        InitObstaclePool();
    }

    private void InitCoinPool() {
        if (coinPref == null) { Debug.LogError("coinPref is null"); return; }
        for (int i = 0; i < coinAmount; i++) {
            GameObject obj = Instantiate(coinPref);
            coinPool.Enqueue(obj);
            obj.SetActive(false);
        }
        Debug.Log("Initialized CoinPool Successfully");
    }

    private void InitObstaclePool() {
        int wallNumber = Mathf.CeilToInt(obstacleAmount*wallRatio);
        int hallNumber = Mathf.CeilToInt(obstacleAmount*hallRatio);

        for (int i = 0; i < wallNumber; i++) {
            GameObject wall = Instantiate(wallPref);
            obstaclePool.Enqueue(wall);
            wall.SetActive(false);
        }
        for (int i = 0; i < hallNumber; i++) {
            GameObject hall = Instantiate(hallPref);
            obstaclePool.Enqueue(hall);
            hall.SetActive(false);
        }

        QueueExtensions.Shuffle(obstaclePool);
    }

    private GameObject GetCoin() {
        if (coinPool.Count > 0) { //coinPool에 오브젝트가 남아있으면 풀에서 꺼냄
            GameObject obj = coinPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(coinPref); //안남아있으면 새로 만들어서 반환
    }

    private GameObject GetObstacle() {
        if (obstaclePool.Count > 0) {
            GameObject obj = obstaclePool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(wallPref);
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
