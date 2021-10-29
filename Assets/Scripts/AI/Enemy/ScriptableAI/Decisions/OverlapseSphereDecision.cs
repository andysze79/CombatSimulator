using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("PluggableAI/Decisions/OverlapseSphere"))]
public class OverlapseSphereDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckSurrounded(controller);
    }
    private bool CheckSurrounded(StateController controller) {
        RaycastHit hit;

        Debug.DrawLine(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.m_LookRange, Color.green);

        if (Physics.SphereCast(
            controller.eyes.position,
            controller.enemyStats.m_OverlapseSphereRadius,
            controller.eyes.forward,
            out hit,
            0)
            && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Found Player");
            controller.chaseTarget = hit.transform;
            return true;
        }
        else
            return false;
    }    
}
