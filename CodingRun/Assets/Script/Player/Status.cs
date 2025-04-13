using System;
using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("스테이터스")]
    public float maxHP = 100f;      //최대 체력
    public float currentHP;         //현재 체력
    public float moveSpeed = 5f;    //이동 속도

    [Header("자동 체력 감소")]
    [SerializeField] private float hpDecayInterval = 1f;    //체력 감소 주기 (초)     
    private float hpDecayTimer = 0f;                        //체력 감소 타이머

    [Header("충돌 감지")]
    [SerializeField] private float detectRadius = 1f;       //충돌 감지 반경
    [SerializeField] private LayerMask detectLayer;         //충돌 감지 레이어
    [SerializeField] private float detectInterval = 0.1f;   //충돌 감지 주기 (초)
    private float detectTimer = 0f;                         //충돌 감지 타이머

    //델리게이트 이벤트
    public event Action<float, float> OnHPChanged; // 체력 변경 알림
    public event Action OnDie;                     // 사망 알림

    //Animator 연결 (피격 애니메이션용)
    //private Animator animator;

    void Start()
    {
        currentHP = maxHP;
        //animator = GetComponent<Animator>(); // Animator 자동 연결
        OnHPChanged?.Invoke(currentHP, maxHP); // 초기 체력 상태 알림
    }

    void Update()
    {
        HandleHPDecay();                        // 1초마다 체력 감소
        HandleCollisionDetection();             // 0.1초마다 충돌 감지
    }

    // 1초마다 0.5씩 체력 감소
    void HandleHPDecay()
    {
        hpDecayTimer += Time.deltaTime;         // 타이머 증가
        if (hpDecayTimer >= hpDecayInterval)    // 1초가 지났다면
        {
            TakeDamage(0.5f);                   // 체력 감소
            hpDecayTimer = 0f;                  // 타이머 초기화
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
            if (hit.TryGetComponent<Heart>(out Heart heart))                                        // Heart와 충돌시
            {
                Heal(heart.healAmount);                                                             // 체력 회복
                heart.DisableObject();                                                              // Heart 오브젝트 비활성화
            }
            else if (hit.TryGetComponent<Coin>(out Coin coin))                                      // Coin과 충돌시
            {
                GameManager.Instance.Score += coin.coinScore;                                       // 코인 점수 추가
                coin.DisableObject();                                                               // Coin 오브젝트 비활성화
            }
        }
    }

    //데미지 처리 + 피격 애니메이션
    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0f);                                      // 체력 감소 (0보다 작게 깍이지 않음)
        Debug.Log($"피해 받음! -{amount} → 현재 HP: {currentHP}");                           // 현재 체력 출력

        // 데미지가 3 이상이면 피격 애니메이션 실행
      /*  if (amount >= 3f && animator != null)
        {
            animator.SetTrigger("Hit");
        }*/

        OnHPChanged?.Invoke(currentHP, maxHP);                                              // 체력 변경 알림

        if (currentHP <= 0f)                                                                // HP가 0가 되면면   
        {
            OnDie?.Invoke();                                                                // 사망 알림
            Die();                                                                          // 사망 처리
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
        Debug.Log("플레이어 사망! 게임 오버 처리 진행");

        GetComponent<Player>().enabled = false;                                     // 조작 정지

        // 게임 종료 처리
        /*if (GameManager.Instance != null)
            GameManager.Instance.GameOver();*/
    }

    // 씬에서 감지 반경 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}