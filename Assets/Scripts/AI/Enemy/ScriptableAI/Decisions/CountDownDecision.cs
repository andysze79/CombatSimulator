using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Decisions/CountDown")]
public class CountDownDecision : Decision
{
    public float m_CountDownTime;
    public override bool Decide(StateController controller)
    {
        return controller.CheckIfCountDownElapsed(m_CountDownTime);
    }
}
