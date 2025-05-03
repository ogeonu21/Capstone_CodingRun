using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    private Vector3[] spawnPoints = new Vector3[3];
    
    [Header("Speed")]
    [Range(0.1f, 20f)]
    public float objectSpeed = 5f;
    [Range(0.1f, 5f)]
    public float spawnPeriod = 0.8f;
    [Space(20)]
    [Header("Item Transform")]
    public Transform items;
    public Transform spawnPoint1 = null;
    public Transform spawnPoint2 = null;
    public Transform spawnPoint3 = null;
    [Space(19)]
    [Header("State Define")]
    public List<StateData> stateDatas = new();
    private IStageState currentBehaviour;
    private StageState nowState;
    private float coinTimer = 0f;
    private Dictionary<StageState, MonoBehaviour> stateDict = new();
    public Quiz quizManager = null;

    void Awake()
    {
        SetSpawnPoint();
    }

    void Start()
    {
        stateDict = stateDatas.ToDictionary(data => data.stageState, data => data.stateComponent);
        quizManager = FindObjectOfType<Quiz>();
        if (quizManager == null) {Debug.LogError("QuizManager is not assigned!!");}

        nowState = StageState.QUESTION_STATE;
        ChangeState(nowState);
    }

    private void SetSpawnPoint() {
        if (!(spawnPoint1&spawnPoint2&spawnPoint3)) {Debug.LogError("SpawnPoint is not assigned!"); return;}
        spawnPoints[0] = spawnPoint1.position;
        spawnPoints[1] = spawnPoint2.position;
        spawnPoints[2] = spawnPoint3.position;
    }

    private void MoveItems()
    {
        items.position -= new Vector3(0, 0, 5 * Time.deltaTime);
    }

    public IEnumerator SpawnItems() {
        while(true /*이 부분의 조건을 isGameRunning == true로 변경해야함. GameManager와 상의*/) {
            if (nowState == StageState.OBSTACLE_STATE) {
                yield return new WaitForSeconds(spawnPeriod);
                int randomIdx = Random.Range(0,3);
                SpawnItem(ObjectType.COIN, spawnPoints[randomIdx], items);
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return null;
            }
        }
    }

    public IEnumerator ShowQuiz() {
        while (true) {
            if (nowState == StageState.QUESTION_STATE) {
                quizManager.ActivateQuestionCanvas();
                quizManager.ShowQuizUI();
                quizManager.LoadNextQuestion();
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return null;
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        //0.8�� �ֱ�� ���� ����
        MoveItems();
        // currentBehaviour?.UpdateState();
        Debug.Log(nowState);
    }

    private MonoBehaviour SpawnItem(ObjectType type, Vector3? location = null, Transform parent = null) {
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

        currentBehaviour?.Exit();    // 이전 상태 마무리
        currentBehaviour = nextState; // 새 상태로 변경
        currentBehaviour.Enter();     // 새 상태 진입
    }
}
