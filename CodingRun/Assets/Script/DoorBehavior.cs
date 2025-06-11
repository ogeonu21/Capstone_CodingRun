using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [Header("문 설정")]
    [Tooltip("이 문이 나타내는 답변 번호 (1, 2, 3)")]
    public int answerNumber;
    
    private static bool canSelectAnyDoor = true;  // 모든 문이 공유하는 정적 변수
    private float cooldownTime = 0.5f; // 쿨다운 시간 (0.5초)

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
        // Player와의 충돌만 처리하고 선택 가능한 상태일 때만 처리
        if (other.CompareTag("Player") && canSelectAnyDoor)
        {
            // 모든 문 선택 불가능 상태로 변경
            canSelectAnyDoor = false;
            
            // 퀴즈 매니저에 답변을 전달
            Quiz quiz = FindObjectOfType<Quiz>();
            if (quiz != null)
            {
                quiz.SubmitAnswer(answerNumber);
            }
            
            // 쿨다운 후 선택 가능하도록 설정
            Invoke("ResetSelection", cooldownTime);
        }
    }
    
    private void ResetSelection()
    {
        canSelectAnyDoor = true;
    }
} 