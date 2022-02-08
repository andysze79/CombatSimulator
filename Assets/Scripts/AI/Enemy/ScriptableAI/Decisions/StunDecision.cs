using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Stun")]
public class StunDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        controller.enemyLogic.CheckStun(controller.CheckIfCountDownElapsed(controller.enemyStats.m_HitStunDuration));
        return controller.CheckIfCountDownElapsed(controller.enemyStats.m_HitStunDuration);
    }
}
