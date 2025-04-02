using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [Range(1, 10)]
    public int loadNum = 3;
    [SerializeField]
    private Queue<GameObject> roadPool = new Queue<GameObject>();
    public List<GameObject> mapPrefs = new List<GameObject>();
    private List<Quiz> quizList;
    [SerializeField]
    private GameObject currentMap;
    [SerializeField]
    private GameObject nextMap;
    private Quiz currentQuiz;

    public bool checkCorrect() {
        //OnTriggerEnter가 발생했을때, 플레이어와 충돌한 선택지의 answer값이 정답인지 체크하는 함수.
        return true;
    }

    public Quiz RandomQuiz() {
        //Quiz 리스트에서 랜덤으로 하나의 Quiz 뽑아서 dequeue (같은 문제가 뽑히지 않도록 삭제) 한 후, 그 값을 return하는 함수.    
        return new Quiz();
    }

    public void InitObjectPool() {
        if (mapPrefs == null) {
            Debug.LogError("mapPref is null!");
            return;
        }

        for (int i = 0; i < loadNum; i++) {
            GameObject road = Instantiate(mapPrefs[0]);
            road.SetActive(true);
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
            Debug.Log("Failed to find Collider Component. trying to find in child Object...");
            collider = obj.GetComponentInChildren<Collider>();
            if (collider == null) {
                Debug.LogError("collider is null! check the component!");
                return new Vector3(0f, 0f, 0f);
            }
        }
        return collider.bounds.size;
    }

    private void InitMap() {
        
    }

    void Start()
    {   
        InitObjectPool(); //Road 오브젝트 풀 로드
    }

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
