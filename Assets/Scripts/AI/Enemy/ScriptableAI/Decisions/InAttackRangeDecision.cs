using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Decisions/InAttackRange")]
public class InAttackRangeDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckInAttackRange(controller);
    }
    private bool CheckInAttackRange(StateController controller)
    {
        var decisionResult = (Vector3.Distance(controller.chaseTarget.position, controller.transform.position) < controller.enemyStats.m_AttackRange);
        return decisionResult;
    }
}
