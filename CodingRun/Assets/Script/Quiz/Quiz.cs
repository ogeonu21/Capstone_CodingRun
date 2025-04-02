using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Question")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("UI")]
    [SerializeField] GameObject questionCanvas;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if(timer.timeUp)
        {
            timer.timeUp = false;
            questionCanvas.SetActive(false);
        }
    }

    void Start()
    {
        GetRandomQuestion();
        DisplayQuestion();
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0,questions.Count);
        currentQuestion = questions[index];

        if(questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }

    void DisplayQuestion()
    {
        timer.isAnsweringQuestion = true;
        questionText.text = currentQuestion.GetQuestion();
    }


}
