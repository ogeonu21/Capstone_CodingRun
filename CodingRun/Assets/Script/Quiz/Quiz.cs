using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 퀴즈 시스템 관리자
// 문제 출제, 타이머 관리, UI 표시 담당
public class Quiz : MonoBehaviour
{
    #region UI 컴포넌트
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI questionText;   // 문제 텍스트 UI
    [SerializeField] private Image timerImage;               // 타이머 게이지
    [SerializeField] private GameObject questionCanvas;      // 문제 표시 캔버스
    private GameObject questionPanel;                        // 문제 패널
    [SerializeField] private TextMeshProUGUI[] answerTexts;  // 답안 선택지 텍스트
    [SerializeField] private Button skipButton;              // Skip 버튼
    #endregion

    #region 프라이빗 필드
    private List<QuestionSO> randomQuestions = new List<QuestionSO>();  // 랜덤하게 섞인 문제 POOL
    private QuestionSO currentQuestion;                           // 현재 문제
    private Timer timer;                                          // 타이머
    private GameManager gameManager;                              // 게임 매니저
    private Status playerStatus;                                  // 플레이어 상태
    #endregion

    #region Unity 라이프사이클
    private void Awake()
    {
        InitializeComponents();
        LoadQuestions();
        ActivateQuestionCanvas();
        ShowQuizUI();
        LoadNextQuestion();  // 첫 문제 로드

        // 첫 문제 로드 후 타이머 시작
        if (timer != null)
        {
            timer.ResetTimer();
            timer.SetQuestionTime(GetCurrentQuestionTimeLimit());
            timer.StartTimer();
        }

        // Skip 버튼 클릭 이벤트 연결
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
    }

    private void Start()
    {
        SubscribeToTimerEvents();
    }

    private void OnEnable()
    {
        Debug.Log($"Quiz OnEnable - 씬: {gameObject.scene.name}, 오브젝트: {gameObject.name}");
    }

    private void OnDisable()
    {
        Debug.Log($"Quiz OnDisable - 씬: {gameObject.scene.name}, 오브젝트: {gameObject.name}");
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            // 게임 오버 상태일 때 타이머 일시정지
            if (timer != null)
            {
                timer.isAnsweringQuestion = false;
            }
            return;
        }

        UpdateTimer();
        CheckSkipButtonVisibility();  // Skip 버튼 가시성 체크
    }
    #endregion

    #region 초기화
    // 컴포넌트 초기화
    private void InitializeComponents()
    {
        FindTimerComponent();
        FindGameManager();
        FindPlayerStatus();
        ValidateUIComponents();
        FindQuestionPanel();
    }

    // 타이머 컴포넌트 찾기
    private void FindTimerComponent()
    {
        timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogWarning("현재 씬에서 타이머를 찾을 수 없습니다. 다른 씬에서 검색 중...");
            FindTimerInOtherScenes();
            
            if (timer == null)
            {
                Debug.LogError("어떤 씬에서도 타이머를 찾을 수 없습니다! 퀴즈 기능이 제한될 수 있습니다.");
            }
        }
    }

    // 게임 매니저 컴포넌트 찾기
    private void FindGameManager()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("게임 매니저를 찾을 수 없습니다!");
        }
    }

    // 플레이어 상태 컴포넌트 찾기
    private void FindPlayerStatus()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStatus = player.GetComponent<Status>();
            if (playerStatus == null)
            {
                Debug.LogError("플레이어의 Status 컴포넌트를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
        }
    }

    // 다른 씬에서 타이머 찾기
    private void FindTimerInOtherScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject root in rootObjects)
                {
                    Timer foundTimer = root.GetComponentInChildren<Timer>(true);
                    if (foundTimer != null)
                    {
                        timer = foundTimer;
                        Debug.Log($"타이머를 찾았습니다: {scene.name} 씬");
                        return;
                    }
                }
            }
        }
    }

    // UI 컴포넌트 유효성 검사
    private void ValidateUIComponents()
    {
        if (questionCanvas == null) Debug.LogError("문제 캔버스가 할당되지 않았습니다!");
        if (questionText == null) Debug.LogError("문제 텍스트가 할당되지 않았습니다!");
        if (timerImage == null) Debug.LogError("타이머 이미지가 할당되지 않았습니다!");
    }

    // 문제 캔버스 활성화
    private void ActivateQuestionCanvas()
    {
        if (questionCanvas != null)
        {
            questionCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("문제 캔버스가 할당되지 않았습니다!");
        }
    }

    // 문제 로드
    private void LoadQuestions()
    {
        randomQuestions.Clear();
        
        // Resources/Questions 폴더에서 문제 로드
        QuestionSO[] loadedQuestions = Resources.LoadAll<QuestionSO>("Questions");
        if (loadedQuestions != null && loadedQuestions.Length > 0)
        {
            randomQuestions.AddRange(loadedQuestions);
            ShuffleQuestions();
        }
        else
        {
            Debug.LogError("Resources/Questions 폴더에서 문제를 찾을 수 없습니다! 퀴즈가 제대로 작동하지 않을 수 있습니다.");
        }
    }

    // 문제 섞기
    private void ShuffleQuestions()
    {
        int n = randomQuestions.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            QuestionSO value = randomQuestions[k];
            randomQuestions[k] = randomQuestions[n];
            randomQuestions[n] = value;
        }
    }

    private void FindQuestionPanel()
    {
        // Canvas 아래의 Panel 찾기
        if (questionCanvas != null)
        {
            questionPanel = questionCanvas.transform.Find("Panel")?.gameObject;
            if (questionPanel != null)
            {
                Debug.Log("Panel을 찾았습니다: " + questionPanel.name);
            }
            else
            {
                Debug.LogError("Canvas 아래에서 Panel을 찾을 수 없습니다!");
            }
        }
    }
    #endregion

    #region 이벤트 관리
    // 타이머 이벤트 구독
    private void SubscribeToTimerEvents()
    {
        if (timer != null)
        {
            timer.onTimeUp.AddListener(HandleTimeUp);
        }
        else
        {
            Debug.LogWarning("타이머가 없어 이벤트 구독에 실패했습니다.");
        }
    }
    #endregion

    #region UI 관리
    // 퀴즈 UI 표시
    private void ShowQuizUI()
    {
        if (questionCanvas == null) return;
        
        questionCanvas.SetActive(true);
        
        // UI가 활성화되지 않은 경우 부모 객체 확인
        if (!questionCanvas.activeInHierarchy)
        {
            Debug.LogWarning("캔버스가 활성화되지 않았습니다. 부모 객체 확인 중...");
            ActivateParentObjects();
            questionCanvas.SetActive(true);
        }
    }

    // 부모 객체 활성화
    private void ActivateParentObjects()
    {
        Transform parent = questionCanvas.transform.parent;
        while (parent != null)
        {
            if (!parent.gameObject.activeInHierarchy)
            {
                parent.gameObject.SetActive(true);
            }
            parent = parent.parent;
        }
    }
    #endregion

    #region 퀴즈 관리
    // 다음 문제 로드
    public void LoadNextQuestion()
    {
        if (randomQuestions.Count == 0)
        {
            EndQuiz();
            return;
        }

        currentQuestion = randomQuestions[0];
        randomQuestions.RemoveAt(0);
        
        if (currentQuestion != null)
        {
            // 문제 표시
            if (questionText != null)
            {
                questionText.text = currentQuestion.GetQuestion();
                DisplayAnswerChoices();
                UpdateSkipButtonVisibility();  // Skip 버튼 상태 업데이트
            }
            else
            {
                Debug.LogError("문제 텍스트 UI가 null입니다.");
            }
        }
        else
        {
            EndQuiz();
        }
    }

    // 답안 선택지 표시
    private void DisplayAnswerChoices()
    {
        if (currentQuestion == null || answerTexts == null) return;
        
        string[] answers = currentQuestion.GetAnswers();
        
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < answers.Length)
            {
                answerTexts[i].text = answers[i];
                answerTexts[i].gameObject.SetActive(true);
            }
            else
            {
                answerTexts[i].gameObject.SetActive(false);
            }
        }
    }

    // 퀴즈 종료
    private void EndQuiz()
    {
        if (questionText != null)
        {
            questionText.text = "모든 문제가 끝났습니다!";
        }

        // 답변 텍스트 비우기
        if (answerTexts != null)
        {
            foreach (var answerText in answerTexts)
            {
                if (answerText != null)
                {
                    answerText.text = "";
                }
            }
        }

        // 타이머 정지
        if (timer != null)
        {
            timer.isAnsweringQuestion = false;
        }

        Debug.Log("모든 문제가 소진되었습니다. 퀴즈가 종료됩니다.");
    }

    // Skip 버튼 가시성 업데이트
    private void UpdateSkipButtonVisibility()
    {
        if (skipButton != null && currentQuestion != null)
        {
            // Easy 난이도일 때는 버튼을 완전히 숨김
            skipButton.gameObject.SetActive(currentQuestion.Difficulty != QuestionDifficulty.Easy);
            Debug.Log($"Skip 버튼 {(skipButton.gameObject.activeSelf ? "표시" : "숨김")} (난이도: {currentQuestion.Difficulty})");
        }
        else
        {
            Debug.LogError("Skip 버튼 또는 현재 문제가 null입니다!");
        }
    }
    #endregion

    #region 타이머 관리
    // 타이머 UI 업데이트
    private void UpdateTimer()
    {
        if (timerImage != null && timer != null)
        {
            timerImage.fillAmount = timer.fillFraction;
        }
    }

    // 시간 초과 처리
    private void HandleTimeUp()
    {
        if (timer != null)
        {
            timer.timeUp = false;
            timer.ResetTimer();  // 타이머 값만 초기화
        }
        
        // GameManager에게 다음 문제 전환을 알림
        if (gameManager != null)
        {
            gameManager.HandleQuizTransition();
        }
    }
    #endregion

    #region 답변 처리
    // 답변 처리
    public void SubmitAnswer(int answerIndex)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("현재 문제가 없습니다!");
            return;
        }

        // 정답 확인
        int selectedAnswer = answerIndex;
        int correctAnswer = currentQuestion.GetCorrectIndex() + 1;
        bool isCorrect = (selectedAnswer == correctAnswer);
        
        // 결과 로깅
        string resultMessage = isCorrect ? "정답" : "오답";
        Debug.Log($"선택한 답변: {selectedAnswer}, 정답: {correctAnswer}, 결과: {resultMessage}");

        // 정답/오답 처리
        if (isCorrect)
        {
            if (gameManager != null)
            {
                gameManager.Score += 100;
                Debug.Log($"현재 점수: {gameManager.Score}점");
            }
        }
        else
        {
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(25);
                Debug.Log($"오답으로 인한 체력 감소! 현재 체력: {playerStatus.currentHP}");
            }
        }

        // 타이머 정지
        if (timer != null)
        {
            timer.isAnsweringQuestion = false;
        }

        // GameManager에게 다음 문제 전환을 알림
        if (gameManager != null)
        {
            gameManager.HandleQuizTransition();
        }
    }
    #endregion

    // Panel 활성화/비활성화 제어
    public void SetQuestionPanelActive(bool active)
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(active);
            Debug.Log($"Panel {(active ? "활성화" : "비활성화")}");
        }
        else
        {
            Debug.LogError("questionPanel이 null입니다!");
        }
    }

    #region public 메서드
    // 현재 문제의 시간 제한을 반환합니다.
    public float GetCurrentQuestionTimeLimit()
    {
        return currentQuestion?.GetTimeLimit() ?? 15f;  // 문제가 없으면 기본값 15초 반환
    }
    #endregion

    // Skip 버튼 가시성 체크
    private void CheckSkipButtonVisibility()
    {
        if (skipButton != null && currentQuestion != null && timer != null)
        {
            // Easy 난이도이거나 타이머가 9.1초 이하로 남았을 때는 버튼 숨김
            bool shouldHide = currentQuestion.Difficulty == QuestionDifficulty.Easy || 
                            (timer.timerValue <= 9.1f && timer.timerValue > 0);
            
            skipButton.gameObject.SetActive(!shouldHide);
        }
    }

    // Skip 버튼 클릭 이벤트 처리
    private void OnSkipButtonClicked()
    {
        if (timer != null)
        {
            timer.timerValue = 9.1f;
        }
    }
}
