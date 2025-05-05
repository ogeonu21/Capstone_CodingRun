using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopicUIManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;
    public GameObject topicCanvas;

    public GameObject GoTopicCanvas_C;
    public GameObject GoTopicCanvas_CS;

    private GameObject previousCanvas;

    public void LoadTopic(string topicKey, GameObject fromCanvas)
    {
        previousCanvas = fromCanvas;

        string path = "Topic/" + topicKey;
        TopicListSO topicList = Resources.Load<TopicListSO>(path);

        if (topicList == null)
        {
            Debug.LogError("Topic SO not found: " + path);
            return;
        }

        if (topicList.Topics.Count > 0)
        {
            Topic topic = topicList.Topics[0];
            titleText.text = topic.term;
            infoText.text = topic.description;

            topicCanvas.SetActive(true);
            previousCanvas.SetActive(false);
        }
    }

    public void OnClickTopic(string topicKey)
    {
        GameObject fromCanvas = topicKey.StartsWith("C_Laug") ? GoTopicCanvas_C : GoTopicCanvas_CS;
        LoadTopic(topicKey, fromCanvas);
    }

    public void GoBack()
    {
        topicCanvas.SetActive(false);

        if (previousCanvas != null)
        {
            previousCanvas.SetActive(true);
            previousCanvas = null;
        }
    }
}
