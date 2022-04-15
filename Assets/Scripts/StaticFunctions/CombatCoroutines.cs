using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public static class CombatCoroutines
{
    static Coroutine PositionLerpingProcess;
    public static IEnumerator AttackerCombatAssistance(Transform attacker, Vector3 target, float maxDistance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = attacker.position;
        var to = target;
        Vector3 pos;

        if (Vector3.Distance(from, to) > maxDistance)
        {
            var dir = (to - from).normalized;
            to = from + dir * maxDistance;
        }

        while (Time.time - startTime < endTime)
        {
            pos = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            attacker.position = new Vector3(pos.x, attacker.position.y, pos.z);
            yield return null;
        }

        attacker.position = to;

        PositionLerpingProcess = null;
    }
    public static IEnumerator AttackerCombatAssistance(Transform attacker, Collider target, float maxDistance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = attacker.position;
        var to = target.ClosestPoint(attacker.position);
        Vector3 pos;

        if (Vector3.Distance(from, to) > maxDistance) {
            var dir = (to - from).normalized;
            to = from + dir * maxDistance;
        }

        while (Time.time - startTime < endTime)
        {
            pos = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            attacker.position = new Vector3(pos.x, attacker.position.y, pos.z);
            yield return null;
        }

        attacker.position = to;

        PositionLerpingProcess = null;
    }
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
        
        Vector3 pos;

        bool isHitWall = false;
        
        Vector3 offset = target.GetComponent<CapsuleCollider>().center;

        while (Time.time - startTime < endTime)
        {
            isHitWall = CheckHitWall(
                    target, 
                    offset, 
                    CombateSimulator.GameManager.Instance.m_GlobalVariables.HitWallCheckingRayLength, 
                    CombateSimulator.GameManager.Instance.m_GlobalVariables.WallLayer);

            if (isHitWall) { Debug.Log("HitWall"); break; }

            pos = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            target.position = new Vector3(pos.x, target.position.y, pos.z);

            yield return null;
        }
        
        if(!isHitWall)
            target.position = to;

        PositionLerpingProcess = null;
    }
    public static IEnumerator CharacterControllerPositionLerping(Transform attacker, CharacterController target, float pushBackDistance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = target.transform.position;
        var to = from;

        var dir = attacker.forward;
        dir.y = 0;
        dir = dir.normalized;
        to = from + dir * pushBackDistance;
                
        while (Time.time - startTime < endTime)
        {
            target.Move(dir * pushBackDistance * Time.deltaTime);

            yield return null;
        }

        PositionLerpingProcess = null;
    }
    public static IEnumerator PositionLerping(Transform target, float distance, float duration, AnimationCurve movement)
    {
        var startTime = Time.time;
        var endTime = duration;
        var from = target.position;
        var to = from;
        Vector3 pos;

        var dir = target.forward;
        dir.y = 0;
        dir = dir.normalized;
        to = from + dir * distance;

        while (Time.time - startTime < endTime)
        {
            pos = Vector3.Lerp(from, to, movement.Evaluate((Time.time - startTime) / endTime));
            target.position = new Vector3(pos.x, target.position.y, pos.z);
            yield return null;
        }

        target.position = to;

        PositionLerpingProcess = null;
    }
    public static IEnumerator ApplyForce(Transform attacker, Rigidbody target, NavMeshAgent ai, float force, float duration) {
        target.useGravity = true;
        ai.isStopped = true;
        ai.enabled = false;

        var dir = target.transform.position - attacker.position;
        dir.y = 0;
        dir = dir.normalized;

        target.AddForce(dir * force, ForceMode.VelocityChange);

        yield return new WaitForSeconds(duration);

        target.useGravity = false;
        target.velocity = Vector3.zero;
        target.angularVelocity = Vector3.zero;

        ai.enabled = true;
        ai.isStopped = false;
    }
    public static bool CheckHitWall(Transform obj, Vector3 offset, float dist, LayerMask layer) {
        Ray ray = new Ray();
        ray.origin = obj.position + offset;
        ray.direction = obj.forward;
        RaycastHit[] hitInfo = new RaycastHit[0];
        int percision = 30;

        for (int i = 0; i < 360 / percision; i++)
        {            
            ray.direction = Quaternion.AngleAxis(percision * i, obj.up) * ray.direction;
            //Physics.RaycastNonAlloc(ray, hitInfo, dist, layer) > 0
            if (Physics.Raycast(ray, dist, layer))
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * dist, Color.red);
                return true;
            }
            else { 
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * dist, Color.white);            
            }
        }

        return false;
    }
}
