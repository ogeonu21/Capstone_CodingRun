using UnityEngine;
using System.Collections;

public class FireworkManager : MonoBehaviour
{
    // Effect 오브젝트의 자식 폭죽 이펙트들을 Inspector에서 할당
    public GameObject[] fireworks;

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
        // 3초 후 자동 비활성화
        StartCoroutine(DisableFireworksAfterDelay(3f));
    }

    private IEnumerator DisableFireworksAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAllFireworksActive(false);
    }
} 