using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문제의 난이도를 정의하는 열거형
public enum QuestionDifficulty
{
    Easy,       // 쉬운 난이도
    Medium,     // 중간 난이도
    Hard        // 어려운 난이도
}

// 퀴즈 문제를 정의하는 ScriptableObject 클래스
// Unity 에디터에서 문제를 생성하고 관리할 수 있음
[CreateAssetMenu(menuName = "QuizQuestion", fileName = "new Question")]
public class QuestionSO : ScriptableObject
{
    #region 문제 기본 정보
    [Header("문제 기본 정보")]
    [SerializeField] private int questionNumber;                    // 문제 번호
    [SerializeField] private QuestionDifficulty difficulty = QuestionDifficulty.Medium;  // 문제 난이도
    #endregion

    #region 문제 내용
    [Header("문제 내용")]
    [TextArea(2, 6)]
    [SerializeField] private string question = "Enter question";    // 문제 텍스트
    [SerializeField] private string[] answers = new string[3];      // 답안 배열
    [SerializeField] private int correctAnswerIndex;                // 정답 인덱스
    #endregion

    #region Public 접근자 메서드
    // 문제 텍스트를 반환합니다.
    public string GetQuestion()
    {
        return question;
    }

    // 모든 답안 선택지를 반환합니다.
    public string[] GetAnswers()
    {
        return answers;
    }

    // 지정된 인덱스의 답안을 반환합니다.
    public string GetAnswer(int index)
    {
        if (index < 0 || index >= answers.Length)
        {
            Debug.LogWarning($"Invalid answer index: {index}");
            return string.Empty;
        }
        return answers[index];
    }

    // 정답의 인덱스를 반환합니다.
    public int GetCorrectIndex()
    {
        return correctAnswerIndex;
    }

    // 문제 번호를 반환합니다.
    public int GetQuestionNumber()
    {
        return questionNumber;
    }

    // 문제의 난이도를 반환합니다.
    public QuestionDifficulty GetDifficulty()
    {
        return difficulty;
    }
    #endregion

    #region 유효성 검사
    // 문제 데이터의 유효성을 검사합니다.
    private void OnValidate()
    {
        ValidateQuestionData();
    }

    // 문제 데이터의 유효성을 검사하고 필요한 경우 경고를 출력합니다.
    private void ValidateQuestionData()
    {
        if (string.IsNullOrEmpty(question))
        {
            Debug.LogWarning($"Question {questionNumber}: Question text is empty");
        }

        if (correctAnswerIndex < 0 || correctAnswerIndex >= answers.Length)
        {
            Debug.LogWarning($"Question {questionNumber}: Invalid correct answer index");
        }

        for (int i = 0; i < answers.Length; i++)
        {
            if (string.IsNullOrEmpty(answers[i]))
            {
                Debug.LogWarning($"Question {questionNumber}: Answer {i} is empty");
            }
        }
    }
    #endregion
}
