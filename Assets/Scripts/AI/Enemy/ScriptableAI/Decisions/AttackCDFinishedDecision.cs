using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = ("PluggableAI/Decisions/AttckCDFinished"))]
public class AttackCDFinishedDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return !controller.enemyStats.AttackCD;
    }
}
