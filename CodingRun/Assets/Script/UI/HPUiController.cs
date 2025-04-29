using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUiController : MonoBehaviour
{
    [SerializeField] private Image hpFillImage;

    public void Bind(Status status)
    {
        if (status != null)
            status.OnHPChanged += UpdateHP;
    }

    private void UpdateHP(float current, float max)
    {
        hpFillImage.fillAmount = current / max;
    }
}
