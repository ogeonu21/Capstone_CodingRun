using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

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
    public event Action<float> OnScoreChanged;
    public event Action<int> OnCoinChanged;

    // --- 전역 상태 ---
    private float score;
    public float Score
    {
        get => score;
        set
        {
            score = value;
            OnScoreChanged?.Invoke(score); // 점수 변경될 때마다 이벤트 호출
        }
    }

    private int inGameCoin;
    public int InGameCoin
    {
        get => inGameCoin;
        set
        {
            inGameCoin = value;
            OnCoinChanged?.Invoke(inGameCoin); // 이벤트 호출
        }
    }

    public int TotalCoin
    {
        get => PlayerPrefs.GetInt("TotalCoin", 0);
        set
        {
            PlayerPrefs.SetInt("TotalCoin", value);
            PlayerPrefs.Save();
        }
    }

    public float HighScore { get; private set; }

    // --- 점수 관련 ---
    private float pointPerSec = 3.1f;

    // --- 타이머 관련 ---
    private float startTime;
    private float timer;
    public float Timer => timer;
    private bool isTimerRunning = false;

    // --- 퀴즈 관련 ---
    private Quiz quizManager;  // 퀴즈 매니저 참조
    [SerializeField] private float quizTransitionDelay = 10f;  // 퀴즈 전환 딜레이 (초)

    // --- 게임 오버 플래그 ---
    public bool IsGameOver { get; set; } = false;

    // --- HPUI 관련 ---
    [SerializeField] private Status playerStatus;
    [SerializeField] private UIManager uiManager;

    // --- State 관련 ---
    [SerializeField] public StageManager stageManager;
 
    // --- 구글 플레이 관련 ---
    //[SerializeField] private string leaderboardId = "CgkIvrSJ8r8EEAIQAA";
    
    // MonoSingleton 의 Awake 호출
    protected override void Awake()
    {
        base.Awake();

        Debug.Log("구글 게임 서비스 초기화 완료");
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
        GameStart();
    }

    private void GameStart()
    {
        Debug.Log("Start 메소드 호출됨");
        
        // 구글 플레이 게임즈 플랫폼 초기화
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        // 로그인 시도 (silent = 자동 로그인 시도)
        SignGooglePlayGames(silent: true);

        // 저장된 최고 점수 로드
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);

        // UIManager 자동 연결
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
                Debug.LogError("UIManager를 찾을 수 없습니다!");
        }

        // Status 자동 연결
        if (playerStatus == null)
        {
            playerStatus = FindObjectOfType<Status>();
            if (playerStatus == null)
                Debug.LogError("Status를 찾을 수 없습니다!");
        }

        if (stageManager == null)
        {
            stageManager = FindObjectOfType<StageManager>();
            if (stageManager == null)
                Debug.LogError("StageManager를 찾을 수 없습니다!");
        }

        // HP UI 바인딩
        if (uiManager != null && playerStatus != null)
        {
            uiManager.Bind(playerStatus);
        }

        //SignGooglePlayGames(true);
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
            // 2025.05.18 : 오건우
            // -> 기존 점수 증가 체계가 미미하여 점수 증가량을 고정값으로 변경. 
            Score += pointPerSec * Time.deltaTime;
            //Debug.Log($"score : {Score:F10}");
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

    //Coin
     public void AddCoin()
    {
        InGameCoin += 1;
    }

    public void AddScore(float amount)
    {
        Score += amount;
    }

    // 구글 로그인

    // 구글 로그인 (silent=true: 자동, false: 수동)
    public void SignGooglePlayGames(bool silent)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.Log("이미 로그인된 상태입니다.");
            return;
        }

        if (silent)
        {
            Debug.Log("자동 로그인 시도");
            PlayGamesPlatform.Instance.Authenticate(OnSignIn);
        }
        else
        {
            Debug.Log("수동 로그인 시도");
            PlayGamesPlatform.Instance.ManuallyAuthenticate(OnSignIn);
        }
    }


    // Authenticate / ManuallyAuthenticate 콜백
    private void OnSignIn(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("Google Play Games 로그인 성공!");
            Debug.Log("환영합니다, " + PlayGamesPlatform.Instance.GetUserDisplayName() + "!");
            // …로그인 성공 후 로직…
        }
        else
        {
            Debug.LogWarning("Google Play Games 로그인 실패: " + status);
            // …실패 처리(자동→수동 버튼 노출 등)…
        }
    }


    // Google Play Games 로그아웃 함수
    public void SignOutGooglePlayGames()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            //PlayGamesPlatform.Instance.SignOut();
            Debug.Log("Google Play Games 로그아웃 성공");
        }
    }

    // 로그인 상태 확인
    public bool IsSignedInToGooglePlayGames()
    {
        return PlayGamesPlatform.Instance.IsAuthenticated();
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

            RecordScore((long)HighScore, false);
        }
    }

    // 점수 등록
    public void RecordScore(long HighScore, bool UI = false)
    {
        Social.ReportScore(HighScore, GPGSIds.leaderboard_score, (bool success) => {
            if (success)
            {
                Debug.Log("Leader Good");
            }
        });
    }

    // 전체 리더보드 표시
    public void ShowLeaderboardUI()
    {
        if (!Social.localUser.authenticated)
        {
            Debug.LogWarning("로그인되지 않아 리더보드를 열 수 없습니다.");
            return;
        }

        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_score);
    }

    // 게임 종료 처리
    
    public void GameOver()
    {
        if (!IsGameOver) return;

        isTimerRunning = false;  // 타이머 정지
        Time.timeScale = 0f;     // 게임 시간 정지

        SaveHighScore();
        Debug.Log($"GameOver() 호출됨 — 플레이 시간: {timer:F2}초");

        // 모든 Rigidbody 일시정지
        foreach (var rb in FindObjectsOfType<Rigidbody>())
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 게임오버 ui 출력 (ui 담당 정헌용 작성)
        FindObjectOfType<Gameover>().ShowGameOverUI(GameManager.Instance.Score, GameManager.Instance.timer);

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
            // StageManager가 없으면 찾아보기
            if (stageManager == null)
            {
                stageManager = FindObjectOfType<StageManager>();
                if (stageManager == null)
                {
                    Debug.LogError("StageManager를 찾을 수 없습니다!");
                    return;
                }
            }

            //Stage가 문제 Stage일때만 활성화
            if (stageManager.getNowState() == StageState.QUESTION_STATE)
            {
                // Panel 비활성화
                quizManager.SetQuestionPanelActive(false);

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
        else
        {
            Debug.LogError("퀴즈 매니저가 없어서 전환을 처리할 수 없습니다!");
        }
    }

    // 타이머 상태에 따른 퀴즈 패널 제어
    public void SetQuizPanelByTimerState(bool isTimerRunning)
    {
        // Quiz가 없으면 다시 찾아보기
        if (quizManager == null)
        {
            FindQuizManager();
        }

        if (quizManager != null)
        {
            quizManager.SetQuestionPanelActive(isTimerRunning);
            Debug.Log($"타이머 {(isTimerRunning ? "시작" : "정지")}에 따라 퀴즈 패널 {(isTimerRunning ? "활성화" : "비활성화")}");
        }
        else
        {
            Debug.LogError("퀴즈 매니저가 없어서 패널을 제어할 수 없습니다!");
        }
    }
}