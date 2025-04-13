using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("문 생성 설정")]
    [Tooltip("문 프리팹")]
    public GameObject doorPrefab;
    [Tooltip("문 이동 속도")]
    public float moveSpeed = 5f;
    
    private List<GameObject> activeDoors = new List<GameObject>();
    private Timer timer;
    private bool doorsSpawned = false;
    private Vector3[] spawnPoints = new Vector3[3];
    private Transform items; // 문이 생성될 부모 오브젝트

    void Start()
    {
        // 타이머 컴포넌트 찾기
        timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogError("타이머를 찾을 수 없습니다!");
        }

        // items 오브젝트 찾기
        items = GameObject.Find("Items")?.transform;
        if (items == null)
        {
            Debug.LogError("Items 오브젝트를 찾을 수 없습니다!");
        }

        // 스폰 포인트 초기화 (코인보다 두 배 더 멀리)
        spawnPoints[0] = new Vector3(-4.9f, 1f, 48f);  // 왼쪽
        spawnPoints[1] = new Vector3(0f, 1f, 48f);      // 중앙
        spawnPoints[2] = new Vector3(4.9f, 1f, 48f);    // 오른쪽
    }

    void Update()
    {
        if (timer != null && timer.isAnsweringQuestion)
        {
            // 타이머가 9초 남았을 때 문 생성
            if (timer.timerValue <= 9f && !doorsSpawned)
            {
                SpawnDoors();
                doorsSpawned = true;
            }
            // 타이머가 9초보다 많을 때 doorsSpawned 플래그 리셋
            else if (timer.timerValue > 9f)
            {
                doorsSpawned = false;
            }
        }
    }

    private void SpawnDoors()
    {
        // 기존 문 제거
        foreach (GameObject door in activeDoors)
        {
            if (door != null)
            {
                Destroy(door);
            }
        }
        activeDoors.Clear();

        // 새로운 문 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject door = Instantiate(doorPrefab, spawnPoints[i], Quaternion.identity, items);
            door.name = "Door" + (i + 1); // 문 이름 설정
            
            // Rigidbody 컴포넌트에 속도 설정
            Rigidbody rb = door.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.back * moveSpeed;
            }
            
            activeDoors.Add(door);
        }
    }

    //이 문이 어떤 답을 의미하는지는 문의 이름을 답으로 변경하는식으로 구상함.
    //예를 들어 TiggerEnter가 발생하면 selectedQuiz 필드 값을 gameObject.name으로 변경경
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log($"플레이어가 {gameObject.name} 문과 충돌했습니다!");
            //추가 로직 구현 예를들어 selectedQuiz 필드를 변경한다.
        }
    }
}
