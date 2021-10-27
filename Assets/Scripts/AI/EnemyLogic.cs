using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class EnemyLogic : MonoBehaviour,IDamagable
{
    public UltEvent WhenReceiveDamage;
    Coroutine PositionLerpingProcess { get; set; }
    void IDamagable.OnReceiveDamage(int damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker)
    {
        WhenReceiveDamage?.Invoke();

        if (PositionLerpingProcess != null)
            StopCoroutine(PositionLerpingProcess);

        PositionLerpingProcess = StartCoroutine(PositionLerping(pushBackDistance, duration, movement, attacker));        
    }
    
    private IEnumerator PositionLerping(float pushBackDistance, float duration, AnimationCurve movement, Transform attacker)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = transform.position;
        var to = from;

        var dir = attacker.forward;
        dir.y = 0;
        dir = dir.normalized;
        to = from + dir * pushBackDistance;

        while (Time.time - startTime < endTime)
        {
            transform.position = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        transform.position = to;

        PositionLerpingProcess = null;
    }
}
