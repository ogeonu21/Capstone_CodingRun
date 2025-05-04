using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnifiedObstacle : MonoBehaviour
{
    Status playerStatus = null;

    [Range(1f, 100f)]
    public float Damage = 0f;

    Animation hitAnimation = null;

    private void Awake() {
        playerStatus = FindAnyObjectByType<Status>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerStatus.TakeDamage(Damage);
        }
    }
}
