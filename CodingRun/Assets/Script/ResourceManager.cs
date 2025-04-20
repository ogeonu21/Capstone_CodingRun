using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    [SerializeField]
    private List<Quiz> quizList;
    public List<Quiz> QuizList{
        get { return quizList; }
        private set {quizList = value;}
    }

    public void LoadQuiz() {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/QuizData");

        if (jsonFile == null) {
            Debug.LogError("json is null");
            return;
        }

        quizList = JsonUtility.FromJson<QuizWrapper>("{\"quizzes\":" + jsonFile.text + "}").quizzes;
        if (quizList != null && quizList.Count > 0) {
            Debug.Log(quizList.Count+" Quizzes has been Loaded!");
        } else {
            Debug.LogError("Failed to Load Quiz Data! Check the JsonFile");
        }
    }

    [System.Serializable]
    public class QuizWrapper {
        public List<Quiz> quizzes;
    }
}
