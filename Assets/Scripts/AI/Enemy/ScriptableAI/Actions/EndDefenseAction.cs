using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/EndDefense")]
public class EndDefenseAction : Action
{
    public override void Act(StateController controller)
    {
        controller.enemyLogic.EndDefense();
    }

    public override void Initialize(StateController controller)
    {

    }

    public override void OnExitState(StateController controller)
    {
        
    }

}
