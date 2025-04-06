using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestionDifficulty
{
    Easy,
    Medium,
    Hard
}

[CreateAssetMenu(menuName = "QuizQuestion", fileName = "new Question")]
public class QuestionSO : ScriptableObject
{
    [SerializeField] int questionNumber;
    [SerializeField] QuestionDifficulty difficulty = QuestionDifficulty.Medium;
    
    [TextArea(2,6)]
    [SerializeField] string question = "Enter question";
    [SerializeField] string[] answers = new string[3];
    [SerializeField] int correctAnswerIndex;

    public string GetQuestion(){
        return question;
    }

    public string GetAnswer(int index){
        return answers[index];
    }

    public int GetCorrectIndex(){
        return correctAnswerIndex;
    }

    public int GetQuestionNumber(){
        return questionNumber;
    }

    public QuestionDifficulty GetDifficulty()
    {
        return difficulty;
    }
}
