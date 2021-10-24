using System.Collections;
using System;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void CallWithDelay(this MonoBehaviour mono, Action method, float delay)
    {
        mono.StartCoroutine(DelayCorutine(method, delay));
    }
    static IEnumerator DelayCorutine(Action method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

}
