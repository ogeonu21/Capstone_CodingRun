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

        // UI 켜기
        endScreenCanvas.SetActive(true);

        // 텍스트 업데이트
        endScoreText.text = $"Score: {score:F1}";
        endCoinText.text = $"Play Time: {time}";
    }
}
