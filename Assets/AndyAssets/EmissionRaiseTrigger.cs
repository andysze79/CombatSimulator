using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionRaiseTrigger : TriggerBase
{
    public bool m_TriggerEmissionRaiseSteps;
    public int m_TargetIndex;
    public bool m_Loop;
    public bool m_Flipflop;
    public float m_Duration;
    public bool m_TriggerEnableEvent;
    private void OnEnable()
    {
        TriggerEnter += StartEmission;
    }
    private void OnDisable()
    {
        TriggerEnter -= StartEmission;        
    }
    private void StartEmission(Collider col) {
        if (m_TriggerEmissionRaiseSteps)
        {
            var emissionRaiseSteps = col.GetComponentInChildren<EmissionRaiseSteps>();

            if (emissionRaiseSteps)
                emissionRaiseSteps.RaiseToColor(m_TargetIndex, m_Loop, m_Flipflop, m_Duration);
        }
        if (m_TriggerEnableEvent)
        {
            var enableEvent = col.GetComponentInChildren<EnableEvent>();

            if (enableEvent)
                enableEvent.enabled = true;
        }
    }
}
