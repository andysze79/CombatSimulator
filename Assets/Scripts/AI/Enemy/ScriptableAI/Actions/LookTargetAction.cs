using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/LookTarget")]
public class LookTargetAction : Action
{
    public override void Act(StateController controller)
    {
        LookAtTarget(controller);
    }

    public override void Initialize(StateController controller)
    {
        
    }

    public override void OnExitState(StateController controller)
    {
        
    }

    private void LookAtTarget(StateController controller) {
        if (controller.chaseTarget == null) return;

        var dir = controller.chaseTarget.position - controller.transform.position;
        dir.y = 0;
        dir = dir.normalized;
        
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * controller.enemyStats.m_FacingSpeed);
    }
}
