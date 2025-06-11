using UnityEngine;
using System.Collections;

public class FireworkManager : MonoBehaviour
{
    // Effect 오브젝트의 자식 폭죽 이펙트들을 Inspector에서 할당
    public GameObject[] fireworks;

    private void Start()
    {
        // 게임 시작 후 1초 뒤에 테스트용 폭죽 자동 실행
        // StartCoroutine(TestFireworkAfterDelay());
    }

    private void OnEnable()
    {
        // 시작 시 모든 폭죽 이펙트 비활성화
        SetAllFireworksActive(false);
    }

    private void SetAllFireworksActive(bool isActive)
    {
        foreach (GameObject firework in fireworks)
        {
            firework.SetActive(isActive);
        }
    }

    // 모든 폭죽 이펙트를 실행하는 함수
    public void PlayFireworks()
    {
        Debug.Log("[FireworkManager] 폭죽 실행!");
        SetAllFireworksActive(true);
        foreach (GameObject firework in fireworks)
        {
            ParticleSystem ps = firework.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        // 2초 후 자동 비활성화
        StartCoroutine(DisableFireworksAfterDelay(2f));
    }

    private IEnumerator DisableFireworksAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAllFireworksActive(false);
    }
} 