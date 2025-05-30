using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("문 생성 설정")]
    [Tooltip("문 프리팹")]
    public GameObject doorPrefab;
    [Tooltip("문 이동 속도")]
    public float moveSpeed = 5f;
    private StageManager manager;
    private List<GameObject> activeDoors = new List<GameObject>();
    private Timer timer;
    private bool doorsSpawned = false;
    private Transform items;

    void Start()
    {
        timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogError("타이머를 찾을 수 없습니다!");
        }

        items = GameObject.Find("Items")?.transform;
        if (items == null)
        {
            Debug.LogError("Items 오브젝트를 찾을 수 없습니다!");
        }
        manager = FindAnyObjectByType<StageManager>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (timer != null && timer.isAnsweringQuestion)
        {
            if (timer.timerValue <= 1f && !doorsSpawned && manager.getNowState() == StageState.QUESTION_STATE)
            {
                SpawnDoors();
                doorsSpawned = true;
            }
            else if (timer.timerValue > 1f)
            {
                doorsSpawned = false;
            }
        }

        MapLoader mapLoader = FindObjectOfType<MapLoader>();
        if (mapLoader != null)
        {
            moveSpeed = mapLoader.moveSpeed;
        }
    }

    private void SpawnDoors()
    {
        foreach (GameObject door in activeDoors)
        {
            if (door != null)
            {
                Destroy(door);
            }
        }
        activeDoors.Clear();

        for (int i = 0; i < 3; i++)
        {
            GameObject door = Instantiate(doorPrefab, manager.spawnPoints[i], Quaternion.identity, items);
            door.name = "Door" + (i + 1);
            
            DoorBehavior doorBehavior = door.GetComponent<DoorBehavior>();
            if (doorBehavior != null)
            {
                doorBehavior.answerNumber = i + 1;
            }
            
            activeDoors.Add(door);
        }
    }
} 