using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inGameCoinText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalCoinText;
    [SerializeField] private Image hpFillImage;

    private void Start()
    {
        UpdateScore(GameManager.Instance.Score);
        UpdateInGameCoin(GameManager.Instance.InGameCoin);
        UpdateTotalCoin(GameManager.Instance.TotalCoin);

        GameManager.Instance.OnCoinChanged += UpdateInGameCoin;
        GameManager.Instance.OnScoreChanged += UpdateScore;
    }

    void OnDestroy()
    {
        // 이벤트 해제
        GameManager.Instance.OnScoreChanged -= UpdateScore;
        GameManager.Instance.OnCoinChanged -= UpdateInGameCoin;
    }

    void UpdateScore(float newScore)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {newScore:F1}";
    }

    void UpdateInGameCoin(int newCoin)
    {
        if (inGameCoinText != null)
            inGameCoinText.text = $"Coin: {newCoin}";
    }

    public void UpdateTotalCoin(int totalCoin)
    {
        if (totalCoinText != null)
            totalCoinText.text = $"Total Coin: {totalCoin}";
    }

    public void Bind(Status status)
    {
        if (status != null)
            status.OnHPChanged += UpdateHP;
            UpdateHP(status.currentHP, status.maxHP); // 초기값 강제 적용
    }

    private void UpdateHP(float current, float max)
    {
        hpFillImage.fillAmount = current / max;
    }
}
