using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    public static bool isPaused { get; set; }
    public static event Action<bool> PauseEvent;

    static void Pause(bool _isPaused)
    {
        isPaused = _isPaused;
        PauseEvent?.Invoke(isPaused);
    }
}
