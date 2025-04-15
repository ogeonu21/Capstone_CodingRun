using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 타입을 나타내는 Enum
public enum ItemType
{
    Coin,
    Heart,
}

public class UnifiedItem : MonoBehaviour
{
    public ItemType itemType; // 인스펙터에서 유형 지정

    // 코인용 파라미터
    [Header("코인 설정")]
    public float rotationSpeed = 30f;
    public float coinScore = 100f;

    // 하트용 파라미터
    [Header("하트 설정")]
    public int healAmount = 40;

    private Rigidbody rb;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다.");
            enabled = false;
        }
    }
    protected virtual void Update()
    {
        CheckOutOfBounds();

        // 코인의 경우 회전 효과 적용
        if (itemType == ItemType.Coin)
        {
            RotateObject();
        }
    }
    private void CheckOutOfBounds()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.y < 0)
        {
            ReturnToPool();
        }
    }

    private void RotateObject()
    {
        float rotationSpeed = ConfigManager.Instance.itemConfig.Coin.rotationSpeed;
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 아이템 타입에 따라 다른 행동 수행
            switch (itemType)
            {
                case ItemType.Coin:
                    // 초기 값
                    float baseCoinScore = ConfigManager.Instance.itemConfig.Coin.coinScore;
                    // 증가률 (예: 0.05 -> 초당 5%)
                    float growthRate = ConfigManager.Instance.itemConfig.Coin.growthRate;
                    // 게임 플레이 시간 할당
                    float elapseTime = Time.timeSinceLevelLoad;
                    float scaledCoinScore = baseCoinScore * (1 + growthRate * elapseTime);
                    GameManager.Instance.Score += scaledCoinScore;
                    GameManager.Instance.SaveHighScore();
                    break;
                case ItemType.Heart:
                    Status status = other.GetComponent<Status>();
                    if (status != null)
                    {
                        int healAmount = ConfigManager.Instance.itemConfig.Heart.healAmount;
                        status.Heal(healAmount);
                    }
                    break;
                    // 여기에 다른 아이템 타입 추가 가능
            }
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        switch (itemType)
        {
            case ItemType.Coin:
                if (ObjectPoolManager.Instance != null)
                    ObjectPoolManager.Instance.ReturnObject(GetPoolType(), this);
                else
                    gameObject.SetActive(false);
                break;

            case ItemType.Heart:
                Destroy(gameObject); 
                break;
        }
    }

    // ObjectType과 ItemType이 다르다면 이 메서드에서 매핑해 줄 수 있음
    private ObjectType GetPoolType()
    {
        // 예시: ItemType.Coin -> ObjectType.COIN, ItemType.Heart -> ObjectType.HEART
        switch (itemType)
        {
            case ItemType.Coin:
                return ObjectType.COIN;
            case ItemType.Heart:
                return ObjectType.HEART;
            default:
                return ObjectType.COIN; // 기본값 설정
        }
    }
    protected virtual void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}