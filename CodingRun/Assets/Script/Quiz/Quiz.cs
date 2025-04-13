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
    [SerializeField] private TextMeshProUGUI[] answerTexts;  // 답안 선택지 텍스트
    #endregion

    #region 프라이빗 필드
    private List<QuestionSO> randomQuestions = new List<QuestionSO>();  // 랜덤하게 섞인 문제 POOL
    private QuestionSO currentQuestion;                           // 현재 문제
    private Timer timer;                                          // 타이머
    #endregion

    #region Unity 라이프사이클
    private void Awake()
    {
        InitializeComponents();
        LoadQuestions();
        ActivateQuestionCanvas();
        ShowQuizUI();
        LoadNextQuestion();
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
        UpdateTimer();
    }
    #endregion

    #region 초기화
    // 컴포넌트 초기화
    private void InitializeComponents()
    {
        FindTimerComponent();
        ValidateUIComponents();
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
    private void LoadNextQuestion()
    {
        if (randomQuestions.Count == 0)
        {
            HandleQuizCompletion();
            return;
        }

        currentQuestion = randomQuestions[0];
        randomQuestions.RemoveAt(0);
        
        if (currentQuestion != null)
        {
            DisplayQuestion();
        }
        else
        {
            Debug.LogError("선택된 문제가 null입니다. 퀴즈를 종료합니다.");
            HandleQuizCompletion();
        }
    }

    // 문제 표시
    private void DisplayQuestion()
    {
        if (timer != null)
        {
            timer.isAnsweringQuestion = true;
            // 현재 문제의 시간 설정 적용
            timer.SetQuestionTime(currentQuestion.GetTimeLimit());
        }
        
        if (questionText != null && currentQuestion != null)
        {
            questionText.text = currentQuestion.GetQuestion();
            DisplayAnswerChoices();
        }
        else
        {
            if (questionText == null) Debug.LogError("문제 텍스트 UI가 null입니다.");
            if (currentQuestion == null) Debug.LogError("현재 문제가 null입니다.");
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

    // 퀴즈 완료 처리
    private void HandleQuizCompletion()
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
            timer.ResetTimer();
        }
        
        LoadNextQuestion();
    }
    #endregion
}
