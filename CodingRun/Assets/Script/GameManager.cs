using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    // 전역 변수: 현재 점수
    public float Score { get; set; }

    // 현재까지의 최고 점수 (읽기 전용)
    public float HighScore { get; private set; }

    // 타이머 변수: 게임 진행 시간을 초 단위로 저장
    private float timer = 0f;

    // 타이머가 이미 실행 중인지 확인하는 플래그
    private bool isTimerRunning = false;

    // Awake 함수: 싱글톤 초기화 및 중복 제거, 씬 전환 시 파괴되지 않게 설정
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }
    }

    // Start 함수: 저장된 HighScore 로드
    private void Start()
    {
        HighScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    // Update 함수에서 MapLoader의 존재 여부를 감지하여 타이머 시작
    private void Update()
    {
        // 타이머가 아직 실행되지 않았으면 MapLoader 인스턴스가 존재하는지 확인
        if (!isTimerRunning)
        {
            MapLoader loader = FindObjectOfType<MapLoader>();
            if (loader != null)
            {
                StartTimer();
            }
        }
    }

    // MapLoader가 DontDestroyOnLoad 상태에 들어간 것을 감지하면 호출할 타이머 시작 함수
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            StartCoroutine(TimerRoutine());
            Debug.Log("타이머 시작됨");
        }
    }

    // 1초마다 실행되는 타이머 코루틴
    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            timer++;
            //Debug.Log("진행 시간: " + timer + "초");
        }
    }

    // HighScore 저장 기능: 현재 점수가 기존 최고 점수보다 높으면 저장
    public void SaveHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetFloat("HighScore", HighScore);
            PlayerPrefs.Save();
            Debug.Log("새로운 최고 점수 저장: " + HighScore);
        }
    }

    // GameOver 함수
    public void GameOver()
    {
        Debug.Log("GameOver() 호출됨" + "플레이 시간: " + timer + "초"); 
    }
}
