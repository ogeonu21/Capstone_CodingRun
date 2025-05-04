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
        Instance.StartCoroutine(Instance.TimerCoroutine(seconds, callback));
    }

    private IEnumerator TimerCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}
