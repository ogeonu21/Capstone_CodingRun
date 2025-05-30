using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;  // LINQ 사용을 위한 네임스페이스 추가

// 퀴즈 시스템 관리자
// 문제 출제, 타이머 관리, UI 표시 담당
public class Quiz : MonoBehaviour
{
    #region UI 컴포넌트
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI questionText;   // 문제 텍스트 UI
    [SerializeField] private Image timerImage;               // 타이머 게이지
    [SerializeField] private GameObject questionCanvas;      // 문제 표시 캔버스
    [SerializeField]private GameObject questionPanel;                        // 문제 패널
    [SerializeField] private TextMeshProUGUI[] answerTexts;  // 답안 선택지 텍스트
    [SerializeField] private Button skipButton;              // Skip 버튼
    [SerializeField] private TextMeshProUGUI feedbackText;   // 피드백 텍스트 UI
    [SerializeField] private TextMeshProUGUI qText;          // Q 텍스트 UI
    #endregion

    #region 프라이빗 필드
    private List<QuestionSO> randomQuestions = new List<QuestionSO>();  // 랜덤하게 섞인 문제 POOL
    private QuestionSO currentQuestion;                           // 현재 문제
    private Timer timer;                                          // 타이머
    private GameManager gameManager;                              // 게임 매니저
    private Status playerStatus;                                  // 플레이어 상태
    private string answer;                                     // 랜덤하게 섞기 전, 정답을 저장할 String
    #endregion

    #region Unity 라이프사이클
    private void Awake()
    {
        InitializeComponents();
        LoadQuestions();
        // ActivateQuestionCanvas();  // 초기화 시점에는 캔버스를 활성화하지 않음
        // ShowQuizUI();  // 초기화 시점에는 UI를 표시하지 않음

        // Skip 버튼 클릭 이벤트 연결
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
    }

    private void Start()
    {
        SubscribeToTimerEvents();
        
        // StageManager가 QUESTION_STATE일 때만 UI를 활성화
        if (gameManager != null && gameManager.stageManager != null)
        {
            if (gameManager.stageManager.getNowState() == StageState.QUESTION_STATE)
            {
                ActivateQuestionCanvas();
                ShowQuizUI();
                
                // 첫 문제 로드 후 타이머 시작
                if (timer != null)
                {
                    timer.ResetTimer();
                    timer.SetQuestionTime(GetCurrentQuestionTimeLimit());
                    timer.StartTimer();
                }
            }
        }
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
            questionPanel = questionCanvas.transform.Find("QuizWindow")?.gameObject;
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
        int correctIndex = currentQuestion.GetCorrectIndex();

        answer = answers[correctIndex];
        
        // 답안과 인덱스를 함께 저장할 리스트 생성
        List<(string answer, int originalIndex)> answerList = new List<(string, int)>();
        for (int i = 0; i < answers.Length; i++)
        {
            answerList.Add((answers[i], i));
        }
        
        // 답안 순서 섞기
        for (int i = answerList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, answerList.Count);
            var temp = answerList[i];
            answerList[i] = answerList[randomIndex];
            answerList[randomIndex] = temp;
        }
        
        // 섞인 순서로 답안 표시
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < answerList.Count)
            {
                answerTexts[i].text = answerList[i].answer;
                answerTexts[i].gameObject.SetActive(true);
                
                // 정답 인덱스 업데이트
                if (answerList[i].originalIndex == correctIndex)
                {
                    currentQuestion.SetCorrectIndex(i);
                }
            }
            else
            {
                answerTexts[i].gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"섞인 선택지: {string.Join(", ", answerList.Select(x => x.answer))}");
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
            // 모든 난이도에서 Skip 버튼 표시
            skipButton.gameObject.SetActive(true);
            Debug.Log($"Skip 버튼 표시 (난이도: {currentQuestion.Difficulty})");
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
        // if (gameManager != null)
        // {
        //     gameManager.HandleQuizTransition();
        // }
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
        int selectedAnswer = answerIndex;  // 사용자가 선택한 답변 (1, 2, 3 중 하나)
        int correctAnswer = currentQuestion.GetCorrectIndex() + 1;  // 정답 인덱스 (0, 1, 2)에 1을 더해 1, 2, 3으로 변환
        bool isCorrect = (selectedAnswer == correctAnswer);  // 선택한 답변과 정답 비교
        
        // 결과 로깅
        string resultMessage = isCorrect ? "정답" : "오답";
        Debug.Log($"선택한 답변: {selectedAnswer}, 정답: {correctAnswer}, 결과: {resultMessage}");

        // 정답/오답 처리
        if (isCorrect)
        {
            // 정답일 경우: 난이도, 체력, 플레이타임에 따른 점수 계산
            if (gameManager != null && playerStatus != null)
            {
                // 난이도별 기본 점수
                float baseScore = currentQuestion.Difficulty switch
                {
                    QuestionDifficulty.Easy => 800f,
                    QuestionDifficulty.Medium => 1400f,
                    QuestionDifficulty.Hard => 2100f,
                    _ => 800f
                };

                // 체력 보너스 계산 (70% + 현재체력*0.3%)
                float healthBonus = (70f + (playerStatus.currentHP * 0.3f)) / 100f;

                // 플레이타임 보너스 계산 (게임 플레이타임 * 0.0314)
                float timeBonus = gameManager.Timer * 0.0314f;

                // 최종 점수 계산
                float finalScore = baseScore * healthBonus * timeBonus;
                gameManager.Score += finalScore;

                Debug.Log($"정답! 획득 점수: {finalScore:F0}점 (기본: {baseScore}, 체력보너스: {healthBonus * 100:F1}%, 시간보너스: {timeBonus:F2})");
            }
        }
        else
        {
            // 오답일 경우: 난이도에 따른 체력 감소
            if (playerStatus != null)
            {
                // 난이도별 체력 감소 비율
                float healthReductionRatio = currentQuestion.Difficulty switch
                {
                    QuestionDifficulty.Easy => 0.92f,  // 92% 유지 (8% 감소)
                    QuestionDifficulty.Medium => 0.85f, // 85% 유지 (15% 감소)
                    QuestionDifficulty.Hard => 0.80f,   // 80% 유지 (20% 감소)
                    _ => 0.92f
                };

                // 체력 감소 적용
                float damageHealth = playerStatus.currentHP - playerStatus.currentHP * healthReductionRatio;
                
                playerStatus.TakeDamage(damageHealth);

                // 체력이 5보다 클 경우에만 3초 지연 적용
                if (playerStatus.currentHP > 5)
                {
                    // 문제 텍스트, 타이머, 답안 선택지, Q 텍스트 숨기고 피드백 텍스트 표시
                    if (questionText != null && feedbackText != null)
                    {
                        // 문제 텍스트와 피드백 텍스트 전환
                        questionText.gameObject.SetActive(false);
                        feedbackText.gameObject.SetActive(true);
                        feedbackText.text = $"오답입니다! 체력이 {(damageHealth):F0}만큼 감소했습니다. \n\n 정답은 \"{answer}\"였습니다.";

                        // 타이머 이미지 비활성화
                        if (timerImage != null)
                        {
                            timerImage.gameObject.SetActive(false);
                        }

                        // 답안 선택지들 비활성화
                        if (answerTexts != null)
                        {
                            foreach (var answerText in answerTexts)
                            {
                                if (answerText != null)
                                {
                                    answerText.gameObject.SetActive(false);
                                }
                            }
                        }

                        // Q 텍스트 비활성화
                        if (qText != null)
                        {
                            qText.gameObject.SetActive(false);
                        }
                    }

                    // 3초 후에 상태 변경
                    if (gameManager != null)
                    {
                        TimerExtension.StartTimer(3f, () => {
                            // 피드백 텍스트 숨기고 문제 텍스트, 타이머, 답안 선택지, Q 텍스트 다시 표시
                            if (questionText != null && feedbackText != null)
                            {
                                feedbackText.gameObject.SetActive(false);
                                questionText.gameObject.SetActive(true);

                                // 타이머 이미지 활성화
                                if (timerImage != null)
                                {
                                    timerImage.gameObject.SetActive(true);
                                }

                                // 답안 선택지들 활성화
                                if (answerTexts != null)
                                {
                                    foreach (var answerText in answerTexts)
                                    {
                                        if (answerText != null)
                                        {
                                            answerText.gameObject.SetActive(true);
                                        }
                                    }
                                }

                                // Q 텍스트 활성화
                                if (qText != null)
                                {
                                    qText.gameObject.SetActive(true);
                                }
                            }
                            gameManager.stageManager.ChangeState(StageState.OBSTACLE_STATE);
                        });
                    }
                }
                else
                {
                    // 체력이 5 이하일 경우 즉시 상태 변경
                    if (gameManager != null)
                    {
                        gameManager.stageManager.ChangeState(StageState.OBSTACLE_STATE);
                    }
                }
            }
        }

        // 정답일 경우 즉시 상태 변경
        if (isCorrect && gameManager != null)
        {
            gameManager.stageManager.ChangeState(StageState.OBSTACLE_STATE);
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

    // 현재 문제의 정답 인덱스를 반환합니다.
    public int GetCurrentQuestionCorrectIndex()
    {
        return currentQuestion?.GetCorrectIndex() ?? 0;  // 문제가 없으면 기본값 0 반환
    }
    #endregion

    // Skip 버튼 가시성 체크
    private void CheckSkipButtonVisibility()
    {
        if (skipButton != null && currentQuestion != null && timer != null)
        {
            // 타이머가 2초 이하로 남았을 때, 또는 타이머가 끝났을 때는 버튼 숨김
            bool shouldHide = timer.timerValue <= 2f ||
                            timer.timeUp ||
                            !timer.isAnsweringQuestion;  // 타이머가 실행 중이 아닐 때도 숨김
            
            skipButton.gameObject.SetActive(!shouldHide);
        }
    }

    // Skip 버튼 클릭 이벤트 처리
    private void OnSkipButtonClicked()
    {
        if (timer != null)
        {
            timer.timerValue = 2f;
        }
    }
}
