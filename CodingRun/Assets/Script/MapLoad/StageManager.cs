using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoint = new Vector3[3];
    public float objectSpeed = 5f;
    private float coinTimer = 0f;
    public float waitingCoinTime = 0.8f;
    public GameObject testPrefab1;
    public Transform items;

    void Awake()
    {
        spawnPoint[0] = new Vector3(-4.9f, 1f, 14f);
        spawnPoint[1] = new Vector3(0f, 1f, 14f);
        spawnPoint[2] = new Vector3(4.9f, 1f, 14f);
    }

    void Start()
    {
    
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        //0.8�� �ֱ�� ���� ����
        coinTimer += Time.deltaTime;
        if (coinTimer > waitingCoinTime) {
            SpawnItem(testPrefab1);
            coinTimer = 0.0f;
        }
        
    }

    private void SpawnItem(GameObject obj)
    {
        GameObject spawnObject = Instantiate(obj, spawnPoint[Random.Range(0, 3)], Quaternion.identity, items);
        spawnObject.GetComponent<Rigidbody>().velocity = Vector3.back * objectSpeed;
    }
}
