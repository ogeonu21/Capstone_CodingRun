using System;
using System.Collections;
using UnityEngine;

public class StaticTimer : MonoBehaviour
{
    private static StaticTimer instance;

    private static StaticTimer Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("StaticTimer");
                instance = go.AddComponent<StaticTimer>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public static void StartTimer(float seconds, Action callback)
    {
        Instance.StartCoroutine(Instance.SafeTimerCoroutine(seconds, callback));
    }

    private IEnumerator SafeTimerCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);

        if (callback != null)
        {
            var target = callback.Target as UnityEngine.Object;
            if (target == null || target.Equals(null))
            {
                Debug.LogWarning("대상 오브젝트가 파괴됨됨");
                yield break;
            }

            try 
            {
                callback.Invoke();
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning("콜백 실행 중 파괴된 참조 발생 - 실행 취소\n" + e.Message);
            }
        }
    }
}
