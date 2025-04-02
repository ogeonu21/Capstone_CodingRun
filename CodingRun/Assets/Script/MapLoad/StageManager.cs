using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoint;
    private float objectSpeed;
    private float coinTimer;
    private float waitingCoinTime;

    [SerializeField]
    public GameObject testPrefab1;

    void Awake()
    {
        
        objectSpeed = 5.0f;
        coinTimer = 0.0f;
        waitingCoinTime = 0.8f;


        spawnPoint = new Vector3[3];


        //임시 spawnPoint 설정
        spawnPoint[0] = new Vector3(-4.9f, 1f, 14f);
        spawnPoint[1] = new Vector3(0f, 1f, 14f);
        spawnPoint[2] = new Vector3(4.9f, 1f, 14f);
    }

    void Start()
    {
    
    }

    void Update()
    {
        //0.8초 주기로 코인 생성
        coinTimer += Time.deltaTime;
        if (coinTimer > waitingCoinTime) {
            SpawnCoin();
            coinTimer = 0.0f;
        }
        
    }

    private void SpawnCoin()
    {
        GameObject spawnObject = Instantiate(testPrefab1, spawnPoint[Random.Range(0, 3)], Quaternion.identity);
        spawnObject.GetComponent<Rigidbody>().velocity = Vector3.back * objectSpeed;
    }
}
