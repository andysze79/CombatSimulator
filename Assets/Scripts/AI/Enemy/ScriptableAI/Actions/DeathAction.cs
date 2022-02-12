using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Death")]
public class DeathAction : Action
{
    public override void Act(StateController controller)
    {
        if (DelayToDeath(controller)) {
            Destroy(controller.gameObject);
        }
    }

    public override void Initialize(StateController controller)
    {
        
    }

    public override void OnExitState(StateController controller)
    {
        
    }

    private bool DelayToDeath(StateController controller) {
        return controller.CheckIfCountDownElapsed(controller.enemyStats.m_DelayToDeathDuration);
    }
}
