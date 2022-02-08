using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ArriveGuardSpot")]
public class ArriveGuardSpotDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool arrive = CheckArriveGuardSpot(controller);
        return arrive;
    }
    private bool CheckArriveGuardSpot(StateController controller) {
        return controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending;
    }
}
