using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveCoroutines
{
    public static IEnumerator PositionLerping(Transform target, Transform to, Vector3 posOffset, float duration, AnimationCurve movement, System.Action<Collider> endEvents, Collider col)
    {
        var startTime = Time.time;
        var endTime = duration;
        var fromPos = target.position;
        var toPos = to.position + posOffset;

        while (Time.time - startTime < endTime)
        {
            target.position = Vector3.Lerp(fromPos, toPos, movement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        target.position = toPos;

        endEvents?.Invoke(col);
    }
    public static IEnumerator PositionLerping(Transform target, Transform to, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var fromPos = target.position;
        var toPos = to.position;

        while (Time.time - startTime < endTime)
        {
            target.position = Vector3.Lerp(fromPos, toPos, movement.Evaluate((Time.time - startTime) / endTime));
            yield return null;
        }

        target.position = toPos;
    }
}
