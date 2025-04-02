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

    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

}
