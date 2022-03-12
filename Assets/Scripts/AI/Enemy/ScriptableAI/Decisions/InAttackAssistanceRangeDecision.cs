using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/InAttackAssistanceRangeDecision")]
public class InAttackAssistanceRangeDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckInAttackAsistanceRange(controller);
    }
    private bool CheckInAttackAsistanceRange(StateController controller)
    {
        if (controller.chaseTarget == null) return false;

        var decisionResult = (Vector3.Distance(controller.chaseTarget.position, controller.transform.position) < controller.enemyStats.m_AttackAssistRange);
        return decisionResult;
    }
}
