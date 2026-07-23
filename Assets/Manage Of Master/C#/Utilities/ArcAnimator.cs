using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcAnimator : MonoBehaviour
{
    public static ArcAnimator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void Animate(Transform t, Transform target, float duration, float delay, float height, Action callback)
    {
        instance.animateInternal(t, target, duration, delay, height, callback);    
    }

    private void animateInternal(Transform t, Transform target, float duration, float delay, float height, Action callback)
    {
        StartCoroutine(MoveAlongArc(t, target, duration, delay, height, callback));
    }

    private IEnumerator MoveAlongArc(Transform t, Transform target, float duration, float delay, float height, Action callback)
    {
        yield return new WaitForSeconds(delay);
        if (t==null)
        {
            //callback?.Invoke();
            yield break;
        }
        float timer = 0;
        Vector3 start = t.position;
        
        while (timer<duration)
        {
            float percent = timer / duration;
            Vector3 pos = Vector3.Lerp(start, target.position, percent);
            pos.y += Mathf.Sin(percent * Mathf.PI) * height;
            t.position = pos;
            
            timer += Time.deltaTime;
            yield return null;
        }
        t.position = target.position;
        callback?.Invoke();
    }
}
