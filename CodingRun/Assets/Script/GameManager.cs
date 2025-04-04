using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    // 전역 변수: 현재 점수
    public int Score { get; set; }

    // 현재까지의 최고 점수 (읽기 전용)
    public int HighScore { get; private set; }

    // Awake 함수: 싱글톤 초기화 및 중복 제거, 씬 전환 시 파괴되지 않게 설정
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }
    }

    // Start 함수: 저장된 HighScore 로드
    private void Start()
    {
        // PlayerPrefs를 통해 이전에 저장된 최고 점수가 있으면 로드 (없으면 0으로 초기화)
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    // HighScore 저장 기능: 현재 점수가 기존 최고 점수보다 높으면 저장
    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save(); // 저장된 값을 디스크에 기록
            Debug.Log("새로운 최고 점수 저장: " + HighScore);
        }
    }
}

