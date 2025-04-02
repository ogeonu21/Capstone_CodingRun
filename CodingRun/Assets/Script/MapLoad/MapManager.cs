using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public List<GameObject> mapPrefs;
    private List<Quiz> quizList;
    [SerializeField]
    private GameObject currentMap;
    [SerializeField]
    private GameObject nextMap;
    private Quiz currentQuiz;

    public bool checkCorrect() {
        //OnTriggerEnter가 발생했을때, 플레이어와 충돌한 선택지의 answer값이 정답인지 체크하는 함수.
        return true;
    }

    public Quiz RandomQuiz() {
        //Quiz 리스트에서 랜덤으로 하나의 Quiz 뽑아서 dequeue (같은 문제가 뽑히지 않도록 삭제) 한 후, 그 값을 return하는 함수.    
        return new Quiz();
    }

    public void StartMap() {
        //처음 시작할때 맵을 불러오는 함수.
    }

    public void RefreshMap() {
        //안보이는 곳을 삭제하고, 새로 보일 맵을 생성하는 함수. (무한 맵)
    }

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
