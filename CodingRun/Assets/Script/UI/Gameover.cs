using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameover : MonoBehaviour
{
    [SerializeField] private GameObject endScreenCanvas;
    [SerializeField] private TextMeshProUGUI endScoreText;
    [SerializeField] private TextMeshProUGUI endCoinText;

                                                             
    public void ShowGameOverUI(float score, float time)
    {
        //coin 출력할때 주석 풀어주시면 됩니다
        //int coin = GameManager.Instance.InGameCoin; //  coin 변수 이거 주석처리 되어 있으면 코인 출력 안됩니다!!



        // UI 켜기
        endScreenCanvas.SetActive(true);

        // 텍스트 업데이트
        endScoreText.text = $"Score: {score:F1}";
        endCoinText.text = $"Playtime: {Mathf.RoundToInt(time)}s";//(코인 출력하실때 이 코드 주석 처리하면 됩니다)
     //endCoinText.text = $"Coin: {coin}"; //코인 출력하게 싶다면 이 코드 주석 해제하고 위 코드도 주석 해제하면 됩니다 (5월18일 ui 정헌용 작성)

      

    }
}
