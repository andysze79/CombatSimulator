using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatCoroutines
{
    static Coroutine PositionLerpingProcess;
    public static IEnumerator PositionLerping(Transform attacker, Transform target, float pushBackDistance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = target.position;
        var to = from;

        var dir = attacker.forward;
        dir.y = 0;
        dir = dir.normalized;
        to = from + dir * pushBackDistance;

        while (Time.time - startTime < endTime)
        {
            target.position = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        target.position = to;

        PositionLerpingProcess = null;
    }
    public static IEnumerator PositionLerping(Transform target, float distance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = target.position;
        var to = from;

        var dir = target.forward;
        dir.y = 0;
        dir = dir.normalized;
        to = from + dir * distance;

        while (Time.time - startTime < endTime)
        {
            target.position = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        target.position = to;

        PositionLerpingProcess = null;
    }
}
