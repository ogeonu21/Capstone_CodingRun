using System;
using UnityEngine;

public static class TimerExtension
{
    public static void StartTimer(float seconds, Action callback)
    {
        StaticTimer.StartTimer(seconds, callback);
    }
}

