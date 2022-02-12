using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : BaseCollectable
{
    [SerializeField]protected float m_HealAmount;
    protected override void Awake()
    {
        base.Awake();
        WhenMoveToPlayer += HealTarget;
    }
    protected void HealTarget(Collider col) {
        col.transform.parent.TryGetComponent<PlayerLogic>(out PlayerLogic player);
        player.OnHeal(m_HealAmount);
    }
}
