using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticGameController : MonoBehaviour
{
    public static StaticGameController Instance = new StaticGameController();

    public event Action onStateChange;

    public void ChangeMode()
    {
        onStateChange?.Invoke();
    }
}
