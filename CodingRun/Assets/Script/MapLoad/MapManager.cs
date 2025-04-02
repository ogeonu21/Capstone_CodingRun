using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Player Setting")]
    [Space(5)]
    public Transform player;
    public float moveSpeed = 5f;


    [Header("Map Setting")]
    [Space(5)]
    public List<GameObject> mapPrefs = new List<GameObject>();
    [Range(1, 10)] public int loadNum = 3;
    public int delayDistance = 5;
    public Vector3 startPos = new Vector3(0f, 0f, 0f);
    public Transform roads;
    

    private Queue<GameObject> roadPool = new Queue<GameObject>();

    private List<Quiz> quizList;
    private Quiz currentQuiz;

    private bool checkCorrect() {
        //OnTriggerEnter가 발생했을때, 플레이어와 충돌한 선택지의 answer값이 정답인지 체크하는 함수.
        return true;
    }

    private Quiz RandomQuiz() {
        //Quiz 리스트에서 랜덤으로 하나의 Quiz 뽑아서 dequeue (같은 문제가 뽑히지 않도록 삭제) 한 후, 그 값을 return하는 함수.    
        return new Quiz();
    }

    /// <summary>
    /// Initialize road's ObejctPool.
    /// </summary>
    private void InitObjectPool() {
        if (mapPrefs == null) {
            Debug.LogError("mapPref is null!");
            return;
        }

        for (int i = 0; i < loadNum; i++) {
            GameObject road = Instantiate(mapPrefs[0], roads);
            road.SetActive(false);
            roadPool.Enqueue(road);
        }

        Debug.Log("RoadPool is initialized!");
    }

    /// <summary>
    /// returns GameObejct's Vector3 size data.
    /// </summary>
    /// <param name="obj">GameObject what want to get size</param>
    /// <returns>Size Vector3 Data</returns>
    private Vector3 GetObjectSize(GameObject obj) {
        Collider collider = obj.GetComponent<Collider>();
        if (collider == null) {
            collider = obj.GetComponentInChildren<Collider>();
            if (collider == null) {
                Debug.LogError("collider is null! check the component!");
                return new Vector3(0f, 0f, 0f);
            }
        }
        return collider.bounds.size;
    }

    /// <summary>
    /// Update infinite scroll map
    /// </summary>
    private void UpdateRoad() {
    
        GameObject firstRoad = roadPool.Peek(); //queue의 첫번째 요소 잠시 가져오기 (dequeue아님)
        
        if (player.transform.position.z > firstRoad.transform.position.z + GetObjectSize(firstRoad).z + delayDistance) //플레이어가 어느정도 왔을때
        {
            roadPool.Dequeue(); // 첫 번째 Road 제거
            float newZ = roadPool.Peek().transform.position.z + GetObjectSize(firstRoad).z * roadPool.Count;
            
            firstRoad.transform.position = new Vector3(firstRoad.transform.position.x, firstRoad.transform.position.y, newZ);
            firstRoad.SetActive(true);
            roadPool.Enqueue(firstRoad); // 다시 큐에 추가
        }
    }

    /// <summary>
    /// Moves Road.
    /// </summary>
    private void MoveRoads()
    {
        foreach (GameObject road in roadPool)
        {
            road.transform.position -= new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Arrange Roads in the roadPool queue.
    /// </summary>
    private void SetRoads() {
        if (roadPool.Count < loadNum) {
            Debug.LogError("Not enough roads in the pool!");
            return;
        }
        
        float currentZ = startPos.z + roadPool.Peek().transform.position.z;
        
        for (int i = 0; i < loadNum; i++) {
            GameObject road = roadPool.Dequeue();
            road.SetActive(true);
            float roadLength = GetObjectSize(road).z; // Road의 길이
            road.transform.position = new Vector3(road.transform.position.x, road.transform.position.y, currentZ);

            roadPool.Enqueue(road);
            currentZ += roadLength; // 다음 로드의 위치를 현재 로드의 끝부분으로 설정
        }
    }

    void Start()
    {   
        InitObjectPool(); //Road 오브젝트 풀 로드
        SetRoads();
    }

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        } 
        //Singleton setting
    }

    void Update()
    {
        UpdateRoad();
        MoveRoads();
    }
}
