using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문제의 난이도를 정의하는 열거형
public enum QuestionDifficulty
{
    Easy,       // 쉬움
    Medium,     // 보통
    Hard        // 어려움
}

// 퀴즈 문제 데이터를 저장하는 ScriptableObject 클래스
// Unity 에디터에서 문제를 생성하고 관리할 수 있습니다.
[CreateAssetMenu(menuName = "QuizQuestion", fileName = "new Question")]
public class QuestionSO : ScriptableObject
{
    #region 문제 설정
    [Header("문제 내용")]
    [TextArea(2,6)]
    [SerializeField] private string question = "Enter question";    // 질문 텍스트
    [SerializeField] private int questionNumber;                    // 문제 번호

    [Header("답안 설정")]
    [SerializeField] private string[] answers = new string[3];      // 답변 옵션 배열 (3개의 선택지)
    [SerializeField] private int correctAnswerIndex;                // 정답의 인덱스

    [Header("난이도 설정")]
    [SerializeField] private QuestionDifficulty difficulty = QuestionDifficulty.Medium;  // 문제 난이도
    [SerializeField] private float timeLimit = 8f;                  // 문제 풀이 제한 시간 (초)
    #endregion

    #region 프로퍼티
    public QuestionDifficulty Difficulty => difficulty;  // 난이도 프로퍼티
    #endregion

    #region 유효성 검사
    // Unity 에디터에서 값이 변경될 때 호출되는 함수
    // 문제 데이터의 유효성을 검사하고 난이도에 따른 시간을 설정합니다.
    private void OnValidate()
    {
        ValidateQuestionData();
        SetTimeByDifficulty();
    }

    // 문제 데이터의 유효성을 검사합니다.
    private void ValidateQuestionData()
    {
        // 답안 배열 크기 검사
        if (answers.Length != 3)
        {
            Debug.LogWarning($"문제 {questionNumber}: 답안은 정확히 3개여야 합니다.");
            System.Array.Resize(ref answers, 3);
        }

        // 정답 인덱스 범위 검사
        if (correctAnswerIndex < 0 || correctAnswerIndex >= answers.Length)
        {
            Debug.LogWarning($"문제 {questionNumber}: 정답 인덱스가 유효하지 않습니다. (0-2 사이여야 함)");
            correctAnswerIndex = Mathf.Clamp(correctAnswerIndex, 0, answers.Length - 1);
        }
    }

    // 난이도에 따라 문제 풀이 시간을 설정합니다.
    private void SetTimeByDifficulty()
    {
        switch (difficulty)
        {
            case QuestionDifficulty.Easy:
                timeLimit = 10f;    // 쉬움: 10초
                break;
            case QuestionDifficulty.Medium:
                timeLimit = 15f;     // 보통: 15초
                break;
            case QuestionDifficulty.Hard:
                timeLimit = 20f;     // 어려움: 20초
                break;
        }
    }
    #endregion

    #region 데이터 접근자
    // 문제 텍스트를 반환합니다.
    public string GetQuestion()
    {
        return question;
    }

    // 모든 답안을 배열로 반환합니다.
    public string[] GetAnswers()
    {
        return answers;
    }

    // 정답 인덱스를 반환합니다.
    public int GetCorrectIndex()
    {
        return correctAnswerIndex;
    }

    // 정답 인덱스를 설정합니다.
    public void SetCorrectIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < answers.Length)
        {
            correctAnswerIndex = newIndex;
        }
        else
        {
            Debug.LogWarning($"정답 인덱스 설정 실패: 유효하지 않은 인덱스 ({newIndex})");
        }
    }

    // 문제 난이도를 반환합니다.
    public QuestionDifficulty GetDifficulty()
    {
        return difficulty;
    }

    // 문제 풀이 제한 시간을 반환합니다.
    public float GetTimeLimit()
    {
        return timeLimit;
    }
    #endregion
}
