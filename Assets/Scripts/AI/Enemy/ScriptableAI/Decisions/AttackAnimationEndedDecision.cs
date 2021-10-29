using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackAnimationEnded")]
public class AttackAnimationEndedDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.CheckCurrentAnimationEnded("Attack");
        //return controller.CheckIfCountDownElapsed(3);
    }
}
