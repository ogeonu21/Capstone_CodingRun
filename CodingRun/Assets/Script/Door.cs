using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //이 문이 어떤 답을 의미하는지는 문의 이름을 답으로 변경하는식으로 구상함.
    //예를 들어 TiggerEnter가 발생하면 selectedQuiz 필드 값을 gameObject.name으로 변경경
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            //추가 로직 구현 예를들어 selectedQuiz 필드를 변경한다.
        }
    }
}
