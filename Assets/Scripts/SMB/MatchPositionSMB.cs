using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MatchPositionSMB : StateMachineBehaviour
{
    [SerializeField] AvatarTarget targetBodyPart = AvatarTarget.Root;
    [SerializeField][MinMaxSlider("Min",1, true)] Vector2 effectiveRange;

    [Header("Assist Settings")]
    [SerializeField, Range(0, 1)] float assistPower = 1;
    [SerializeField, Range(0, 10)] float assistDistance = 1;

    public IMatchTarget target;

    MatchTargetWeightMask weightMask;
    bool isSkip = false;
    bool isInitialized = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (target.TargetPosition == Vector3.positiveInfinity) return;

        if (!isInitialized) 
        {
            var weight = new Vector3(assistPower, 0, assistPower);
            weightMask = new MatchTargetWeightMask(weight, 0);
            isInitialized = true;
        }

        isSkip = Vector3.Distance(target.TargetPosition, animator.rootPosition) > assistDistance;
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isSkip || animator.IsInTransition(layerIndex) || target.TargetPosition == Vector3.positiveInfinity)
            return;

        if (stateInfo.normalizedTime > effectiveRange.y)
        {
            animator.InterruptMatchTarget(false);
        }
        else 
        {
            animator.MatchTarget(
                target.TargetPosition,
                animator.bodyRotation,
                targetBodyPart,
                weightMask,
                effectiveRange.x, effectiveRange.y);   
        }
    }
}
public interface IMatchTarget { 
    Vector3 TargetPosition { get;}
}
