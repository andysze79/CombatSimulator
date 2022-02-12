using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : Action
{
    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    public override void Initialize(StateController controller)
    {
        
    }

    public override void OnExitState(StateController controller)
    {
        
    }

    private void Chase(StateController controller) {
        if (controller.chaseTarget == null) return;

        controller.navMeshAgent.destination = controller.chaseTarget.position;
        controller.navMeshAgent.isStopped = false;
    }
}
