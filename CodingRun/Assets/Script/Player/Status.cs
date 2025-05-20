using System;
using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
    [Header("스탯 설정")]
    public float maxHP = 100f;
    public float currentHP;


    [Header("체력 자연 감소")]
    public float hpDecreaseInterval = 1f;
    public float hpDecreaseAmount = 0.5f;

    [Header("충돌 판정")]
    public float detectRadius = 1.5f;
    public LayerMask detectLayer;
    public float detectInterval = 0.1f;

    private float detectTimer = 0f;
    private float decreaseTimer = 0f;
    private bool isDead = false;


    public event Action<float, float> OnHPChanged;
    public event Action OnDie;

    private Animator animator;

    public StageManager stagemanager;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP); // 초기 체력 상태 알림
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return; // 게임 오버 상태라면 업데이트 중지

        HandleHPDecay();                        // 1초마다 체력 감소
        HandleCollisionDetection();             // 0.1초마다 충돌 감지
    }

    // 1초마다 0.5씩 체력 감소
    void HandleHPDecay()
    {
        decreaseTimer += Time.deltaTime;               // 타이머 증가

        if (decreaseTimer >= hpDecreaseInterval)
        {
            TakeDamage(hpDecreaseAmount);
            decreaseTimer = 0f;
        }

    }

    // 0.1초마다 주변 충돌 감지
    void HandleCollisionDetection()
    {
        detectTimer += Time.deltaTime;                  // 타이머 증가
        if (detectTimer >= detectInterval)              // 0.1초가 지났다면
        {
            DetectNearbyObjects();                      // 주변 물체 감지
            detectTimer = 0f;                           // 타이머 초기화
        }
    }

    // 충돌 대상 수동 감지
    void DetectNearbyObjects()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, detectLayer);     // 주변 물체 감지

        foreach (Collider hit in hits)                                                              // 감지된 물체에 대해 반복
        {
            if (hit.TryGetComponent<UnifiedItem>(out UnifiedItem item))
            {
                switch (item.itemType)
                {
                    case ItemType.Coin:
                        //Debug.Log("코인과 충돌 확인됨!");
                        float baseCoinScore = ConfigManager.Instance.itemConfig.Coin.coinScore;
                        float growthRate = ConfigManager.Instance.itemConfig.Coin.growthRate;
                        float elapsedTime = Time.timeSinceLevelLoad;
                        int cycle = stagemanager != null ? stagemanager.cycleNum : 0;
                        float multiplier = Mathf.Min(1.0f + 0.1f * cycle, 2.0f);
                        float scaledScore = Mathf.RoundToInt(baseCoinScore * multiplier);
                        // GameManager.Instance.Score += scaledScore;
                        //GameManager.Instance.SaveHighScore();

                        GameManager.Instance.AddCoin();
                        GameManager.Instance.AddScore(scaledScore);
                        Debug.Log($"[코인 획득] +{scaledScore:F1}점 → 총 점수: {GameManager.Instance.Score:F1}");
                        item.ReturnToPool(); // 풀로 반환
                        break;

                    case ItemType.Heart:
                        int healAmount = ConfigManager.Instance.itemConfig.Heart.healAmount;
                        Heal(healAmount);
                        Destroy(item.gameObject);
                        break;
                }
            }
        }
    }

    //데미지 처리 + 피격 애니메이션
    public void TakeDamage(float amount)
    {
        if (isDead) return; // 이미 죽은 상태라면 데미지 처리 안함

        currentHP = Mathf.Max(currentHP - amount, 0f);                                      // 체력 감소 (0보다 작게 깍이지 않음)
        //Debug.Log($"피해 받음! -{amount} → 현재 HP: {currentHP}");                           // 현재 체력 출력

        // 데미지가 현재체력의 5% 이상, 절대값이 1이상이면 피격 애니메이션 실행
        if (amount >= currentHP * 0.05f && amount >= 1f && HitEffectManager.Instance != null)
        {
            if (animator != null)
                animator.SetTrigger("Hit");

            HitEffectManager.Instance.ShowHitEffect();
        }

        OnHPChanged?.Invoke(currentHP, maxHP); //uiManager.UpdateHP(currentHP, maxHP) 실행                                             // 체력 변경 알림

        if (currentHP <= 0f)
        {
            isDead = true; // 죽음 처리
            OnDie?.Invoke();
            Die();
        }
    }

    //체력 회복 처리
    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);                                       // 체력 회복 (최대 체력 초과 방지)
        Debug.Log($"HP 회복! +{amount} → 현재 HP: {currentHP}");                                 // 현재 체력 출력
        OnHPChanged?.Invoke(currentHP, maxHP);                                                  // 체력 변경 알림
    }

    //사망 처리 (조작 정지 + GameManager 호출)
    private void Die()
    {
        Debug.Log("플레이어 사망! Die 애니메이션 실행");

        if (animator != null)
            animator.SetTrigger("Die");

        // 게임 정지용 플래그는 즉시 설정 → 맵, 오브젝트 정지
        GameManager.Instance.IsGameOver = true;

        // GameOver는 애니메이션 재생 후 지연 호출
        StartCoroutine(WaitAndGameOver(2.18f)); // 애니메이션 길이에 맞게 조절

        // 조작 정지
        if (TryGetComponent<Player>(out var player))
            player.enabled = false;
    }
    
    private IEnumerator WaitAndGameOver(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // TimeScale 영향 없음
        GameManager.Instance.GameOver();
    }

    // 씬에서 감지 반경 시각화
    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }*/
}