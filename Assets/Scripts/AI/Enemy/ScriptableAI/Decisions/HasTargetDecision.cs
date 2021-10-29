using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Decisions/HasTarget")]
public class HasTargetDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.chaseTarget;
    }
}
