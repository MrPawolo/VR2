using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticGameController 
{
    public static StaticGameController Instance = new StaticGameController();

    /// <summary>
    /// changing level state to opposite
    /// </summary>
    public event Action onStateChange;
    public void ChangeMode() => onStateChange?.Invoke();


    public event Action onLevelFinished;
    public void OnLevelFinishedCall() => onLevelFinished?.Invoke();
    
    
    public event Action onSpikesPlayerEnter;
    public void OnSpikesPlayerEnterCall() => onSpikesPlayerEnter?.Invoke();

    
    
    public event Action onLevelStart;
    public void OnLevelStart() => onLevelStart?.Invoke();

    public event Action onLevelUnload;
    public void OnLevelUnload() => onLevelUnload?.Invoke();

}
