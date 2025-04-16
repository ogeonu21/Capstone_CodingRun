using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance;

    [Header("비네트 이미지")]
    public Image vignetteImage;

    [SerializeField] private float effectDuration = 0.5f;
    private float timer = 0f;
    private bool isActive = false;

    void Awake()
    {
        Instance = this;
        if (vignetteImage != null)
            vignetteImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= effectDuration)
            {
                vignetteImage.gameObject.SetActive(false);
                isActive = false;
                timer = 0f;
            }
        }
    }

    public void ShowHitEffect()
    {
        if (vignetteImage == null) return;

        vignetteImage.gameObject.SetActive(true);
        timer = 0f;
        isActive = true;
    }
}

