using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance; //캔버스 싱글톤 생성
    public Stack<GameObject> canvasStack = new Stack<GameObject>(); //캔버스 스택 생성

    void Awake()
    {
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject mainCanvas = GameObject.Find("MainWindow");
        if(mainCanvas != null){
            canvasStack.Push(mainCanvas);
        }
    }

    public void OpenCanvas(GameObject newCanvas){ //버튼 클릭 시 버튼에 해당하는 gameobject 열도록 하는 메소드
        if(canvasStack.Count >0){
            canvasStack.Peek().SetActive(false); //Peek(): 맨 위에 객체 제거하지 않고 반환, SetActive(): false이면 해당 캔버스 비활성화하여 씬에서 보이지 않게된다.
        }
        newCanvas.SetActive(true);
        canvasStack.Push(newCanvas); //canvasStack에 newCanvas push
    }

    public void CloseCurrentCanvas(){
         //닫는 버튼 구현시 현재 캔버스 닫도록 하는 메소드
        Time.timeScale = 1f;
        if(canvasStack.Count > 0){ 
            GameObject currentCanvas = canvasStack.Pop(); //currentCanvas변수에 스택에서 제거된 최상단 현재 활성화된 캔버스가 할당
            currentCanvas.SetActive(false);

            if(canvasStack.Count >0){
                canvasStack.Peek().SetActive(true);
            }
        }

    }

    public void OnClickPaused(GameObject pausedPanel){
        Time.timeScale = 0f;
        if(canvasStack.Count > 0){
            canvasStack.Peek().SetActive(false);
        }
        pausedPanel.SetActive(true);
        canvasStack.Push(pausedPanel);

        // 문제 스테이지이고 게임 오버가 아닐 때만 문제 패널 비활성화
        if (GameManager.Instance != null && GameManager.Instance.stageManager != null && !GameManager.Instance.IsGameOver)
        {
            if (GameManager.Instance.stageManager.getNowState() == StageState.QUESTION_STATE)
            {
                Quiz quiz = FindObjectOfType<Quiz>();
                if (quiz != null)
                {
                    quiz.SetQuestionPanelActive(false);
                }
            }
        }
    }

    public void OnclickResume(GameObject pausedPanel){
        Time.timeScale = 1f;
        pausedPanel.SetActive(false);

        // 문제 스테이지이고 게임 오버가 아닐 때만 문제 패널 다시 활성화
        if (GameManager.Instance != null && GameManager.Instance.stageManager != null && !GameManager.Instance.IsGameOver)
        {
            if (GameManager.Instance.stageManager.getNowState() == StageState.QUESTION_STATE)
            {
                Quiz quiz = FindObjectOfType<Quiz>();
                if (quiz != null)
                {
                    quiz.SetQuestionPanelActive(true);
                }
            }
        }
    }

    public void OnClickRestart()
    {
        
        StartCoroutine(RestartSceneSafely());
    }

    public void OnClickGoToMain(){
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        if (ConfigManager.Instance != null)
        {
            Destroy(ConfigManager.Instance.gameObject);
        }
        if (ObjectPoolManager.Instance != null)
        {
            Destroy(ObjectPoolManager.Instance.gameObject);
        }
        SceneManager.LoadScene("MainScene");
    }
    
    public void ExitGame(){
        Application.Quit();
    }
    
    private IEnumerator RestartSceneSafely()
    {
        Time.timeScale = 1f;

        // GameManager 등 싱글톤이 파괴되지 않도록 관리하려면 필요 시 Destroy 해도 됨
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        if (ConfigManager.Instance != null)
        {
            Destroy(ConfigManager.Instance.gameObject);
        }
        if (ObjectPoolManager.Instance != null)
        {
            Destroy(ObjectPoolManager.Instance.gameObject);
        }



        // 혹시 모르니 1 프레임 대기
        yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OverlayScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        StartCoroutine(InitUI());
        IEnumerator InitUI()
        {
            // 씬 로딩이 완료될 때까지 더 오래 기다립니다
            yield return new WaitForSeconds(0.5f);

            // 씬이 제대로 로드되었는지 확인
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.isLoaded)
            {
                Debug.Log($"Scene {sceneName} loaded successfully");

                // Quiz 컴포넌트를 찾아서 초기화 상태 확인
                Quiz[] quizzes = FindObjectsOfType<Quiz>();
                if (quizzes.Length > 0)
                {
                    Debug.Log($"Found {quizzes.Length} Quiz components in the loaded scene");
                    foreach (Quiz quiz in quizzes)
                    {
                        // Quiz 컴포넌트가 제대로 초기화되었는지 확인
                        if (quiz != null)
                        {
                            Debug.Log("Quiz component is initialized");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"No Quiz components found in the loaded scene {sceneName}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load scene {sceneName}");
            }
        }
    }

    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

}
