using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoint;
    private float objectSpeed;

    [SerializeField]
    public GameObject testPrefab1;

    void Awake()
    {
        //임시 spawnPoint 설정
        objectSpeed = 4.0f;
        spawnPoint = new Vector3[3];

        spawnPoint[0] = new Vector3(-4.9f, 1f, 14f);
        spawnPoint[1] = new Vector3(0f, 1f, 14f);
        spawnPoint[2] = new Vector3(4.9f, 1f, 14f);
    }

    void Start()
    {
    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
    }

    private void SpawnCoin()
    {
        GameObject spawnObject = Instantiate(testPrefab1, spawnPoint[Random.Range(0, 3)], Quaternion.identity);
        spawnObject.GetComponent<Rigidbody>().velocity = Vector3.back * objectSpeed;
    }
}
