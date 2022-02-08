using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthModule
{
    public static void SetUpHealth(IHealthBehavior healthBehavior, Transform target) {
        healthBehavior.CurrentHealth = healthBehavior.MaxHealth;
        healthBehavior.HealthObject = target;
    }
    public static void ChangeHealth(IHealthBehavior healthBehavior, float amount) {
        healthBehavior.CurrentHealth += amount;
        healthBehavior.CurrentHealth = Mathf.Clamp(healthBehavior.CurrentHealth, 0, healthBehavior.MaxHealth);
        healthBehavior.HealthPercentage = healthBehavior.CurrentHealth / healthBehavior.MaxHealth;        
    }
    public static void SwitchHealthBar(IHealthBehavior healthBehavior, bool onOff) {
        UIManager.Instance.HealthbarController.SwitchHealthBar(healthBehavior, onOff);
    }
}
