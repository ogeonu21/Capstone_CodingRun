using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "QuizQuestion", fileName = "new Question")]
public class QuestionSO : ScriptableObject
{

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
}
