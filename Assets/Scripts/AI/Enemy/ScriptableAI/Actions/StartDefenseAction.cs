using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/StartDefense")]
public class StartDefenseAction : Action
{
    public override void Act(StateController controller)
    {
        controller.enemyLogic.StartDefense();
    }

    public override void Initialize(StateController controller)
    {
        
    }

    public override void OnExitState(StateController controller)
    {
        
    }
}
