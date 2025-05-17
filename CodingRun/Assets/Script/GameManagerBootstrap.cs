using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManagerPrefab;

    private static bool isInitialized = false;

    private void Awake()
    {
        // 이미 GameManager 존재하면 아무 작업 안함
        if (GameManager.Instance != null)
        {
            Destroy(gameObject); // Bootstrap도 파괴
            return;
        }

        // GameManager 인스턴스화
        GameManager gm = Instantiate(gameManagerPrefab);
        gm.name = "GameManager (Singleton)";
        DontDestroyOnLoad(gm.gameObject);

        Debug.Log("GameManager 인스턴스화 완료");
    }
}
