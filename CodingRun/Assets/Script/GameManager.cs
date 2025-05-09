using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
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
    [SerializeField] private UIManager uiManager;
 
    // --- 구글 플레이 관련 ---
    [SerializeField] private string leaderboardId = "CgkIvrSJ8r8EEAIQAA";
    
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
        Debug.Log("Start 메소드 호출됨");
        // 저장된 최고 점수 로드
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);

        //HPUI
        uiManager.Bind(playerStatus);
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
   /* public void SignGooglePlayGames(bool silent)
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

            SubmitScoreToLeaderboard((long)HighScore);
        }
    }

    // 구글 리더보드에 점수 제출
    private void SubmitScoreToLeaderboard(long score)
    {
        if(!IsSignedInToGooglePlayGames())
        {
            Debug.LogWarning("로그인 안됨");
            return;
        }

        if (string.IsNullOrEmpty(leaderboardId) || leaderboardId == "CgkIvrSJ8r8EEAIQAA")
        {
            Debug.LogError("리더보드 ID가 설정되지 않음");
            return;
        }
        Debug.LogError("리더보드에 점수 제출 시도");

        PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (success) => 
        {
            if (success)
            {
                Debug.Log("점수 제출");
            }
            else
            {
                Debug.LogWarning("점수 제출 실패");
            }
        });
    }
*/
    // 게임 종료 처리
    
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        isTimerRunning = false;  // 타이머 정지
        Time.timeScale = 0f;     // 게임 시간 정지

        //SaveHighScore();
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
        yield return new WaitForSeconds(3f); //3초 후 시작.
        
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