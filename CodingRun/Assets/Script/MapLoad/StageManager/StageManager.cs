using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoint = new Vector3[3];
    public float objectSpeed = 5f;
    private float coinTimer = 0f;
    [Range(0.1f, 5f)]
    public float spawnPeriod = 0.8f;
    public GameObject testPrefab1;
    public Transform items;
    private IStageState currentBehaviour;
    private StageState nowState;
    public List<StateData> stateDatas = new();
    private Dictionary<StageState, MonoBehaviour> stateDict = new();

    void Awake()
    {
        spawnPoint[0] = new Vector3(-4.9f, 1f, 14f);
        spawnPoint[1] = new Vector3(0f, 1f, 14f);
        spawnPoint[2] = new Vector3(4.9f, 1f, 14f);
    }

    void Start()
    {
        stateDict = stateDatas.ToDictionary(data => data.stageState, data => data.stateComponent);
        
        nowState = StageState.OBSTACLE_STATE;
        ChangeState(nowState);
        StartCoroutine(SpawnItems());
    }

    private void MoveItems()
    {
        MapLoader mapLoader = FindObjectOfType<MapLoader>();
        if (mapLoader != null)
        {
            items.position -= new Vector3(0, 0, mapLoader.moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator SpawnItems() {
        while(nowState == StageState.OBSTACLE_STATE) {
            yield return new WaitForSeconds(spawnPeriod);
            int randomIdx = Random.Range(0,2);
            SpawnItem(ObjectType.COIN, spawnPoint[randomIdx], items);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        //0.8�� �ֱ�� ���� ����
        MoveItems();
    }

    private MonoBehaviour SpawnItem(ObjectType type)
    {
        return ObjectPoolManager.Instance.GetObject(type);
    }

    private MonoBehaviour SpawnItem(ObjectType type, Vector3 location)
    {
        MonoBehaviour obj = SpawnItem(type);
        obj.transform.position = location;
        return obj;
    }

    private MonoBehaviour SpawnItem(ObjectType type, Vector3 location, Transform transform) {
        MonoBehaviour obj = SpawnItem(type, location);
        obj.transform.SetParent(transform);
        return obj;
    }

   public void ChangeState(StageState state) 
    {
        if (!stateDict.TryGetValue(state, out MonoBehaviour stateComponent)) 
        {
            Debug.LogError($"{state}에 해당하는 상태가 없습니다. : "+stateComponent);
            return;
        }

        IStageState nextState = stateComponent as IStageState;
        if (nextState == null) 
        {
            Debug.LogError($"{state}의 MonoBehaviour는 IStageState를 구현하지 않음.");
            return;
        }

        currentBehaviour?.Exit();    // 이전 상태 마무리
        currentBehaviour = nextState; // 새 상태로 변경
        currentBehaviour.Enter();     // 새 상태 진입
    }
}
