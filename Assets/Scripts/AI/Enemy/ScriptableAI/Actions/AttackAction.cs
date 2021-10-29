using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(StateController controller)
    {
        Attack(controller);    
    }
    private void Attack(StateController controller) {
        RaycastHit hit;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.m_AttackRange, Color.red);
        
        var spherecastResult = Physics.SphereCast(
                controller.eyes.position,
                controller.enemyStats.m_LookSphereCastRadius,
                controller.eyes.forward,
                out hit,
                controller.enemyStats.m_AttackRange)
                && hit.collider.CompareTag("Player");

        if (spherecastResult) {
                            
            controller.navMeshAgent.isStopped = true;

            if (!controller.enemyStats.AttackCD) {
                // Call Attack Function
                Debug.Log("Attack");
                controller.StartAttackCD();
            }

            if (controller.CheckIfCountDownElapsed(controller.enemyStats.m_AttackRate)) {
                //controller.tankShooting.Fire(controller.enemyStats.attackForce, controller.enemyStats.attackRate);
            }
        }
    }
}
