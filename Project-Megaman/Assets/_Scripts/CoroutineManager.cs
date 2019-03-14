using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineManager {

    public static void BeginCoroutine(IEnumerator routine, ref Coroutine container, MonoBehaviour parentBehaviour)
    {
        HaltCoroutine(ref container, parentBehaviour);

        if (parentBehaviour.enabled)
        {
            container = parentBehaviour.StartCoroutine(routine);
        }     
    }

    public static void HaltCoroutine(ref Coroutine container, MonoBehaviour parentBehaviour)
    {
        if(container != null)
        {
            parentBehaviour.StopCoroutine(container);
            container = null;
        }
    }
}
