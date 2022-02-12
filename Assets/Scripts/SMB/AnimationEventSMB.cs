using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSMB : StateMachineBehaviour
{
    public int damageTriggerIndex;
    public IAnimationEvent target;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target.OnStateExit(damageTriggerIndex);
    }
}
public interface IAnimationEvent {
    void OnStateExit(int damageTriggerIndex);
}
