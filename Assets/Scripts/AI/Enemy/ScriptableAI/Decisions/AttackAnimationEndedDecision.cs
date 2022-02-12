using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackAnimationEnded")]
public class AttackAnimationEndedDecision : Decision
{
    public string AnimationName = "Attack";
    public float AnimationDuration = 1;
    public override bool Decide(StateController controller)
    {        
        if(controller.CheckIfCountDownElapsed(AnimationDuration)) 
            return true;
        else
            return controller.CheckCurrentAnimationEnded(AnimationName);
    }
}
