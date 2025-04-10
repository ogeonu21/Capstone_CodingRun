using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("스테이터스")]
    public int maxHP = 100;
    public int currentHP;

    public float moveSpeed = 5f;

    private float hpDecayInterval = 2f;
    private float hpDecayTimer = 0f;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        HandleHPDecay();
    }

    void HandleHPDecay()    //2초마다 체력 1씩 감소
    {
        hpDecayTimer += Time.deltaTime;
        if (hpDecayTimer >= hpDecayInterval)
        {
            TakeDamage(1); // 체력 1씩 감소
            hpDecayTimer = 0f;
        }
    }

    public void TakeDamage(int amount)  //데미지 함수
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0); // 체력이 음수가 되지 않게
        Debug.Log($"피해 받음! -{amount} → 현재 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)  //체력 회복 함수
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP); // 체력이 최대값 넘지 않게
        Debug.Log($"HP 회복! +{amount} → 현재 HP: {currentHP}");
    }

    private void Die()  //HP=0면 사망처리 & 조작 비활성화
    {
        Debug.Log("플레이어 사망! 게임 오버 처리 진행");
        //GameManager.Instance.GameOver(); // 게임 종료 처리
        // 플레이어 조작 비활성화
        GetComponent<Player>().enabled = false;
    }

    private void OnTriggerEnter(Collider other) //충돌 판정 함수(내부구현X)
    {
        // 하트 아이템: 체력 회복
        /*if (other.TryGetComponent<Heart>(out Heart heart))
        {
            Heal(heart.healAmount); //heart 스크립트 안에 있는 값만큼 heal함수가 회복함
            heart.DisableObject(); // 아이템 비활성화
        }

        // 코인 아이템: 점수 증가
        else if (other.TryGetComponent<Coin>(out Coin coin))
        {
            GameManager.Instance.Score += coin.coinScore;   //coin 스크립트의 coinscore값만큼 게임 매니저의 score값을 증가시킴.
            coin.DisableObject();   // 아이템 비활성화
        }*/

        //추후 장애물 등 추가
    }
}