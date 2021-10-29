using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetWithinChaseDistance")]
public class TargetWithinChaseDistanceDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckTargetDistance(controller);
    }
    private bool CheckTargetDistance(StateController controller) {
        var decisionResult = (Vector3.Distance(controller.chaseTarget.position, controller.transform.position) < controller.enemyStats.m_ChaseDistance);
        if (!decisionResult) controller.chaseTarget = null;
        return decisionResult;
    }
}
