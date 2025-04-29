using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// 1) 제네릭 MonoSingleton<T> 구현
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// 2) GameManager
public class GameManager : MonoSingleton<GameManager>
{
    // --- 전역 상태 ---
    public float Score { get; set; }
    public float HighScore { get; private set; }

    // --- 타이머 관련 ---
    private float startTime;
    private float timer;
    public float Timer => timer;
    private bool isTimerRunning = false;

    // --- 퀴즈 관련 ---
    private Quiz quizManager;  // 퀴즈 매니저 참조
    [SerializeField] private float quizTransitionDelay = 10f;  // 퀴즈 전환 딜레이 (초)

    // --- 게임 오버 플래그 ---
    public bool IsGameOver { get; private set; } = false;

    // --- HPUI 관련 ---
    [SerializeField] private Status playerStatus;
    [SerializeField] private HPUiController hpUiController;
    
    // MonoSingleton 의 Awake 호출
    protected override void Awake()
    {
        base.Awake();
    }

    private void FindQuizManager()
    {
        // 모든 로드된 씬에서 Quiz 찾기
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    Quiz quiz = obj.GetComponentInChildren<Quiz>(true);
                    if (quiz != null)
                    {
                        quizManager = quiz;
                        Debug.Log($"Quiz를 찾았습니다: {scene.name} 씬");
                        return;
                    }
                }
            }
        }
        Debug.LogWarning("어떤 씬에서도 Quiz를 찾을 수 없습니다!");
    }

    private void Start()
    {
        // 저장된 최고 점수 로드
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);

        //HPUI
        hpUiController.Bind(playerStatus);
    }

    private void Update()
    {
        if (!isTimerRunning)
        {
            // MapLoader 가 씬에 로드되면 자동으로 타이머 시작
            var loader = FindObjectOfType<MapLoader>();
            if (loader != null)
            {
                StartTimer();
                Debug.Log("타이머 시작됨");
            }
        }
        else if (!IsGameOver)
        {
            // Time.timeScale 이 0 이면 Time.time 증가치가 0 이므로
            // 게임이 멈춰 있으면 timer 도 멈춥니다.
            timer = Time.time - startTime;
            //Debug.Log($"진행 시간: {timer:F2}초");
        }
    }

    // 외부에서 타이머를 강제로 시작할 때도 호출 가능
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            startTime = Time.time;
        }
    }

    // 최고 점수 저장
    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetFloat("HighScore", HighScore);
            PlayerPrefs.Save();
            Debug.Log("새로운 최고 점수 저장: " + HighScore);
        }
    }

    // 게임 종료 처리
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        isTimerRunning = false;  // 타이머 정지

        SaveHighScore();
        Debug.Log($"GameOver() 호출됨 — 플레이 시간: {timer:F2}초");

        // 모든 Rigidbody 일시정지
        foreach (var rb in FindObjectsOfType<Rigidbody>())
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 퀴즈 전환 처리
    public void HandleQuizTransition()
    {
        // Quiz가 없으면 다시 찾아보기
        if (quizManager == null)
        {
            FindQuizManager();
        }

        if (quizManager != null)
        {
            StartCoroutine(HandleQuizTransitionWithDelay());
        }
        else
        {
            Debug.LogError("퀴즈 매니저가 없어서 전환을 처리할 수 없습니다!");
        }
    }

    private IEnumerator HandleQuizTransitionWithDelay()
    {
        // Panel 비활성화
        quizManager.SetQuestionPanelActive(false);
        
        // 10초 대기
        yield return new WaitForSeconds(10f);
        
        // Panel 활성화 및 다음 문제 로드
        quizManager.SetQuestionPanelActive(true);
        quizManager.LoadNextQuestion();
        
        // 타이머 시작
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.ResetTimer();
            timer.SetQuestionTime(quizManager.GetCurrentQuestionTimeLimit());
            timer.StartTimer();
        }
    }
}