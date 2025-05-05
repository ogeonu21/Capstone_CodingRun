using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTopicList", menuName = "ScriptableObjects/TopicListSO", order = 1)]
public class TopicListSO : ScriptableObject
{
    public string Title;  // 주제 이름 (예: C언어, 컴퓨터구조 등)
    public List<Topic> Topics;  // 개념 리스트
}

[System.Serializable]
public class Topic
{
    public string topicNumber;   // 번호 (예: C_Laug01)
    public string term;          // 개념 이름 (예: 포인터)
    public string description;   // 설명 (예: 포인터는 주소를 담는 변수다~)
}