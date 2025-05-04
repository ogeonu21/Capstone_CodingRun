using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [Header("문 설정")]
    [Tooltip("이 문이 나타내는 답변 번호 (1, 2, 3)")]
    public int answerNumber;

    void Start()
    {
        // Collider 컴포넌트 확인
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"{gameObject.name}: Collider 컴포넌트가 없습니다!");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogError($"{gameObject.name}: Collider의 Is Trigger가 체크되어 있지 않습니다!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Player와의 충돌만 처리
        if (other.CompareTag("Player"))
        {
            // 퀴즈 매니저에 답변을 전달
            Quiz quiz = FindObjectOfType<Quiz>();
            if (quiz != null)
            {
                quiz.SubmitAnswer(answerNumber);
            }
            StageManager manager = FindAnyObjectByType<StageManager>();
            manager.ChangeState(StageState.OBSTACLE_STATE);
        }
    }
} 