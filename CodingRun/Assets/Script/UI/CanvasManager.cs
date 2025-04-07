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
        GameObject mainCanvas = GameObject.Find("MainCanvas");
        if(mainCanvas != null){
            canvasStack.Push(mainCanvas);
        }
    }

    public void OpenCanvas(GameObject newCanvas){ //버튼 클릭 시 버튼에 해당하는 캔버스 or gameobject 열도록 하는 메소드
        if(canvasStack.Count >0){
            canvasStack.Peek().SetActive(false); //Peek(): 맨 위에 객체 제거하지 않고 반환, SetActive(): false이면 해당 캔버스 비활성화하여 씬에서 보이지 않게된다.
        }
        newCanvas.SetActive(true);
        canvasStack.Push(newCanvas); //canvasStack에 newCanvas push
    }

    public void CloseCurrentCanvas(){ //닫는 버튼 구현시 현재 캔버스 닫도록 하는 메소드
        if(canvasStack.Count > 0){ 
            GameObject currentCanvas = canvasStack.Pop(); //currentCanvas변수에 스택에서 제거된 최상단 현재 활성화된 캔버스가 할당
            currentCanvas.SetActive(false);

            if(canvasStack.Count >0){
                canvasStack.Peek().SetActive(true);
            }
        }

    }
    
    public void OverlayScene(string sceneName){
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
