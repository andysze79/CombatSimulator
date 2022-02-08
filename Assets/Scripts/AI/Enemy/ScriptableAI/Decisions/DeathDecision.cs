using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CheckDeath")]
public class DeathDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckHealth(controller);
    }
    private bool CheckHealth(StateController controller) {        
        return controller.enemyLogic.CurrentHealth <= 0;
    }
}
