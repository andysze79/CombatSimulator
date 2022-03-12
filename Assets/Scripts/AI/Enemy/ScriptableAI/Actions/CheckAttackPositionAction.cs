using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Actions/CheckAttackPosition")]
public class CheckAttackPositionAction : Action
{
    public override void Act(StateController controller)
    {
        
    }

    public override void Initialize(StateController controller)
    {
        controller.enemyLogic.CheckAttackPosition();
    }

    public override void OnExitState(StateController controller)
    {
        
    }
}
