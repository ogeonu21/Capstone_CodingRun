using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPanel : MonoBehaviour
{
[Tooltip("설명용 패널")]
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject[] innerCircle;

    [Tooltip("왼쪽 이동 버튼")]
    [SerializeField] private Button leftButton;

    [Tooltip("오른쪽 이동 버튼")]
    [SerializeField] private Button rightButton;

    private int currentIndex = 0;

    void Start()
    {
        // 버튼에 기능 연결
        leftButton.onClick.AddListener(PreviousPanel);
        rightButton.onClick.AddListener(NextPanel);

        ShowPanel(currentIndex); // 첫 패널 보여줌
    }

    void ShowPanel(int index)
    {
        // 모든 패널 비활성화
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
            innerCircle[i].SetActive(i ==index);
        }

        // 버튼 활성화/비활성화 제어
        leftButton.interactable = index > 0;
        rightButton.interactable = index < panels.Length - 1;
    }

    void NextPanel()
    {
        if (currentIndex < panels.Length - 1)
        {
            currentIndex++;
            ShowPanel(currentIndex);
        }
    }

    void PreviousPanel()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPanel(currentIndex);
        }
    }
}
