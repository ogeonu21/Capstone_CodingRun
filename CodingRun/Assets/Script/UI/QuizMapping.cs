using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;

public class QuizMapping : MonoBehaviour
{
    [Header("문제 리스트 (ScriptableObjects)")]
    public QuestionSO[] questionList;

    [Header("문제 상세 보기 UI")]
    public GameObject quizInfoCanvas;
    public TextMeshProUGUI quizNumText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] choiceTexts; // 보기 텍스트들 (3개)
    public TextMeshProUGUI answerText;
    public Button[] buttonList;


    private readonly string[] titles = new string[]
{
    "상수란?",
    "예약어의 종류",
    "산술연산자의 종류",
    "비트 연산자란?",
    "C언에서 인코딩 방식",
    "실수 자료형이란?",
    "sizeof 연산자란?",
    "조건문의 종류",
    "반복문의 종류",
    "헤더 파일이란?",
    "표준 라이브러리의 종류",
    "자료형: float",
    "서식지정자의 종류",
    "c언어의 여러가지 함수들",
    "함수: scanf()",
    "포인터 형식 1",
    "배열의 값 1",
    "double vs float",
    "변수의 종류",
    "swtich문의 형식",
    "포인터의 형식 2 심화편",
    "배열의 값 2 심화편",
    "c언의 메모리 처리",
    "이건 옳은 형식?",
    "구조체 형식",
    "문자열의 형식",
    "#define 의 식별자"
};


    void Start()
    {
        // Resources/Questions 폴더에서 문제 데이터 자동 로딩
        for (int i = 0; i < buttonList.Length; i++)
    {
    int capturedIndex = i;
    buttonList[i].onClick.AddListener(() => ShowQuiz(capturedIndex));
    }
        
         questionList = Resources.LoadAll<QuestionSO>("Questions")
         .OrderBy(q => int.Parse(Regex.Match(q.name, @"\d+").Value))
         .ToArray();
         answerText.text = "정답:";
         Debug.Log($"로드된 문제 수: {questionList.Length}");
    }
// 파일 이름에서 숫자만 추출 (예: Clanguage_01 → 01)
private string ExtractQuestionNumber(string name)
{
    // 숫자만 추출
    string number = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
    return number;
}
    // 문제 버튼에서 호출되는 함수
    public void ShowQuiz(int index)
    {
        if (index < 0 || index >= questionList.Length)
        {
            Debug.LogWarning("잘못된 인덱스: " + index);
            return;
        }

        QuestionSO q = questionList[index];

        // UI 활성화
        quizInfoCanvas.SetActive(true);

        // 문제 번호 & 제목
        quizNumText.text = $"Q{ExtractQuestionNumber(q.name)}";
       titleText.text = titles[index];


        // 질문 본문
        questionText.text = q.GetQuestion();

        // 보기 출력
        string[] answers = q.GetAnswers();
        for (int i = 0; i < choiceTexts.Length && i < answers.Length; i++)
        {
            choiceTexts[i].text = $"선택지{i + 1}: {answers[i]}";
        }

        // 정답 출력
        answerText.text = $"정답: 선택지 {answers[q.GetCorrectIndex()]}";

    }

    // 문제 닫기 버튼 연결 함수
    public void CloseQuizInfo()
    {
        quizInfoCanvas.SetActive(false);
    }

    // 버튼 이름 기반으로 타이틀 가져오기
    private string GetTitleFromButton(int index)
    {
        GameObject button = GameObject.Find($"Button ({index})");
        if (button != null)
        {
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            return tmp != null ? tmp.text : "";
        }
        return "";
    }
}
