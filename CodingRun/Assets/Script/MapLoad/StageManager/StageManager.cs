using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    public Vector3[] spawnPoints = new Vector3[3];
    //Speed 관련 변수
    [Header("Speed")]
    [Range(0.1f, 20f)]
    public float objectSpeed = 5f;
    private float coinTimer = 0f;
    [Range(0.1f, 5f)]
    public float spawnPeriod = 0.8f;
    [Space(20)]
    //Item 관련 변수
    [Header("Item Transform")]
    public Transform items;
    [SerializeField] private Transform spawnPoint1 = null;
    [SerializeField] private Transform spawnPoint2 = null;
    [SerializeField] private Transform spawnPoint3 = null;
    [Space(19)]

    //State 매치 관련
    [Header("State Define")]
    public List<StateData> stateDatas = new();
    private Dictionary<StageState, MonoBehaviour> stateDict = new();

    //상태 관련 변수
    private IStageState currentBehaviour;
    [SerializeField]
    private StageState nowState;

    //코루틴
    private Coroutine spawnCoroutine = null;
    private Coroutine quizCoroutine = null;

    [Range(1,10)]
    public int heartPerCycle = 5;
    public int cycleNum = 0;    
    public float adjustTime = 3f;
        
    void Awake()
    {
        SetSpawnPoint();
    }

    void Start()
    {
        stateDict = stateDatas.ToDictionary(data => data.stageState, data => data.stateComponent);
        
        nowState = StageState.QUESTION_STATE;
        ChangeState(nowState);
        //IncreaseSpeed(0.01f, 15f);
    }

    private void SetSpawnPoint() {
        if (!(spawnPoint1&spawnPoint2&spawnPoint3)) {Debug.LogError("SpawnPoint is not assigned!"); return;}
        spawnPoints[0] = spawnPoint1.position;
        spawnPoints[1] = spawnPoint2.position;
        spawnPoints[2] = spawnPoint3.position;
    }

    private void MoveItems()
    {
        MapLoader mapLoader = FindObjectOfType<MapLoader>();
        if (mapLoader != null)
        {
            items.position -= new Vector3(0, 0, mapLoader.moveSpeed * Time.deltaTime);
        }
    }

    public void StartSpawn() {
        spawnCoroutine = StartCoroutine(SpawnItems());
    }

    public void StopSpawn() {
        StopCoroutine(spawnCoroutine);
    }

    public IEnumerator SpawnItems() {
        while(true /*이 부분의 조건을 isGameRunning == true로 변경해야함. GameManager와 상의*/) {
            if (nowState == StageState.OBSTACLE_STATE) {
                yield return new WaitForSeconds(spawnPeriod);
                SpawnObject();
            } else {
                yield return null;
            }
        }
    }

    public void SpawnObject()
    {
        // 1. 장애물 라인 랜덤 선택
        int obstacleLine = Random.Range(0, 3);

        // 2. 장애물 종류 결정 (벽 90%, 구멍 10%)
        float rand = Random.value; // 0~1
        MonoBehaviour obstaclePrefab = rand <= 0.8f ? 
        SpawnItem(ObjectType.WALL, spawnPoints[obstacleLine], items) : 
        SpawnItem(ObjectType.HALL, spawnPoints[obstacleLine], items);

        // 3. 장애물 스폰
        

        // 4. 코인 라인 결정 (나머지 두 라인 중 하나)
        List<int> remainingLines = new List<int> { 0, 1, 2 };
        remainingLines.Remove(obstacleLine);
        int coinLine = remainingLines[Random.Range(0, 2)];

        // 5. 코인 스폰
        SpawnItem(ObjectType.COIN, spawnPoints[coinLine], items);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        //0.8�� �ֱ�� ���� ����
        MoveItems();
        currentBehaviour?.UpdateState();
    }

    public MonoBehaviour SpawnItem(ObjectType type, Vector3? location = null, Transform parent = null) {
    var obj = ObjectPoolManager.Instance.GetObject(type);
    if (location.HasValue)
        obj.transform.position = location.Value;
    if (parent != null)
        obj.transform.SetParent(parent);
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
        nowState = state;
        currentBehaviour?.Exit();    // 이전 상태 마무리
        currentBehaviour = nextState; // 새 상태로 변경
        currentBehaviour.Enter();     // 새 상태 진입
    }

    private void IncreaseSpeed(float amount, float maxSpeed) {
        //속도 증가함수
        StartCoroutine(IncreaseSpeed(amount, maxSpeed));

        IEnumerator IncreaseSpeed(float amount, float maxSpeed) {
            while (true /*이 부분 GameManager와 합의의*/) {
                if (objectSpeed >= maxSpeed) yield break;

            objectSpeed += amount;
            MapLoader.instance.moveSpeed += amount;
            
            yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
