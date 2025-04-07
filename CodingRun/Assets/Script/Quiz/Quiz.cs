using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
[Header("UI Components")]
    [SerializeField] TextMeshProUGUI questionText;  
    [SerializeField] Image timerImage;                 
    [SerializeField] GameObject questionCanvas;        

    private List<QuestionSO> questions = new List<QuestionSO>(); // 문제 리스트
    private QuestionSO currentQuestion;                
    private Timer timer;                           


    void Awake()
    {
        timer = FindObjectOfType<Timer>();

        questions.AddRange(Resources.LoadAll<QuestionSO>("Questions"));
    }

    void Start()
    {
        LoadNextQuestion();
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

    void LoadNextQuestion()
    {
        if(questions.Count ==0)
        {
            Debug.Log("모든 문제를 풀었다.");
             return;
        }

        int index = Random.Range(0,questions.Count);
        currentQuestion = questions[index];
        questions.RemoveAt(index);

        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        timer.isAnsweringQuestion = true;
        questionText.text = currentQuestion.GetQuestion();
    }


}
