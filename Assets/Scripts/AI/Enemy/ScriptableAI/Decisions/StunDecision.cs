using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Stun")]
public class StunDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.CheckIfCountDownElapsed(controller.enemyStats.m_HitStunDuration);
    }
}
