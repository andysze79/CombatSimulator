using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool targetvisible = Look(controller);
        return targetvisible;
    }
    private bool Look(StateController controller) {
        RaycastHit hit;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.m_LookRange, Color.green);

        if (Physics.SphereCast(
            controller.eyes.position, 
            controller.enemyStats.m_LookSphereCastRadius, 
            controller.eyes.forward, 
            out hit, 
            controller.enemyStats.m_LookRange)
            && hit.collider.CompareTag("Player"))
        {
            controller.chaseTarget = hit.transform;
            return true;
        }
        else
            return false;
    }

}
