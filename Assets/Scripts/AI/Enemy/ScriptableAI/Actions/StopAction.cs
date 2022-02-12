using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Stop")]
public class StopAction : Action
{
    public override void Act(StateController controller)
    {
        controller.navMeshAgent.isStopped = true;
    }

    public override void Initialize(StateController controller)
    {
       
    }

    public override void OnExitState(StateController controller)
    {
        
    }
}
