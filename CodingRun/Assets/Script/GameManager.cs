using UnityEngine;

// 1) 제네릭 MonoSingleton<T> 구현
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] {typeof(T)} 인스턴스는 앱 종료 중이므로 반환되지 않습니다.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
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

    // --- 게임 오버 플래그 ---
    public bool IsGameOver { get; private set; } = false;

    // MonoSingleton 의 Awake 호출
    public void Init()
    {

    }

    private void Start()
    {
        // 저장된 최고 점수 로드
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    private void Update()
    {
        float elapedTime = Time.deltaTime;

        StageManager.Instance.ElapedUpdate(elapedTime);

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
}