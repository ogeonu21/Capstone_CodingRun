using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoint = new Vector3[3];
    public float objectSpeed = 5f;
    private float coinTimer = 0f;
    public float waitingCoinTime = 0.8f;
    public GameObject testPrefab1;
    public Transform items;
    private IStageState currentState = null;

    public List<StateData> stateDatas = new();

    private Dictionary<StageState, MonoBehaviour> stateDatasDic = new();

    void Awake()
    {
        spawnPoint[0] = new Vector3(-4.9f, 1f, 14f);
        spawnPoint[1] = new Vector3(0f, 1f, 14f);
        spawnPoint[2] = new Vector3(4.9f, 1f, 14f);
    }

    void Start()
    {
        stateDatasDic = stateDatas.ToDictionary(data => data.stageState, data => data.stateComponent);
        Debug.Log("Keys : "+stateDatasDic.Keys);
        Debug.Log("Values : "+stateDatasDic.Values);
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
        currentState?.Update();    
    }

    private Dictionary<StageState, MonoBehaviour> ToDict(List<StateData> list) {
        var dict = new Dictionary<StageState, MonoBehaviour>();
        
        foreach(var data in list) {
            dict.Add(data.stageState, data.stateComponent);
        }
        
        return dict;
    }

    private void SpawnItem(GameObject obj)
    {
        GameObject spawnObject = Instantiate(obj, spawnPoint[Random.Range(0, 3)], Quaternion.identity, items);
        spawnObject.GetComponent<Rigidbody>().velocity = Vector3.back * objectSpeed;
    }

    private void ChangeState(StageState state) {

    }
}
