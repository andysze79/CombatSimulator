using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowDownEffect : MonoBehaviour
{
    public Image m_SlowMoEffect;
    private TimeManager m_TimeManager { get; set; }
    private void Awake()
    {
        m_TimeManager = GetComponent<TimeManager>();
        m_TimeManager.WhenSlowMoStart += SlowMoEffectOn;
        m_TimeManager.WhenSlowMoEnd += SlowMoEffectOff;
        SlowMoEffectOff();
    }
    private void OnDisable()
    {
        m_TimeManager.WhenSlowMoStart -= SlowMoEffectOn;
        m_TimeManager.WhenSlowMoEnd -= SlowMoEffectOff;        
    }
    private void SlowMoEffectOn() {
        m_SlowMoEffect.enabled = true;
    }
    private void SlowMoEffectOff()
    {
        m_SlowMoEffect.enabled = false;
    }
}
