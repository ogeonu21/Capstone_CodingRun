using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameover : MonoBehaviour
{
    [SerializeField] private GameObject endScreenCanvas;
    [SerializeField] private TextMeshProUGUI endScoreText;
    [SerializeField] private TextMeshProUGUI endCoinText;

    private bool isGameOver = false;

    public void ShowGameOverUI(float score, int coin)
    {
        if (isGameOver) return;

        isGameOver = true;

        // UI 켜기
        endScreenCanvas.SetActive(true);

        // 텍스트 업데이트
        endScoreText.text = $"Score: {score:F1}";
        endCoinText.text = $"Coin: {coin}";
    }
}
