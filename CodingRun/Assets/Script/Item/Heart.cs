using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Heart : Item
{
    [Header("회복 체력 (HP)")]
    [Tooltip("하트를 획득했을 때 증가하는 체력 값")]
    public int healAmount = 40; // 하트 획득 시 회복할 체력 양
    void Start()
    {
        
    }

    // 플레이어와 충돌 시 체력을 회복시킴
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            Status status = other.GetComponent<Status>();
            if(status != null){
                status.Heal(healAmount);
            }
            DisableObject();
        }
    }

}
