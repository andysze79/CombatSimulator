using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : TriggerBase
{
    public bool m_UseHitLimit = false;
    public int m_HitCount = 1;
    private int CurrentHitCount { get; set; }

    private void OnEnable()
    {
        if (m_UseHitLimit)
        {
            TriggerEnter += UpdateHitCount;
            CurrentHitCount = 0;
        }
    }
    private void OnDisable()
    {
        if (m_UseHitLimit)
        {
            TriggerEnter -= UpdateHitCount;
        }
    }

    private void UpdateHitCount(Collider other) {
        if (CurrentHitCount < m_HitCount)
            ++CurrentHitCount;
        else
        {
            CurrentHitCount = 0;
            gameObject.SetActive(false); 
        }
    }    
}
