using System;
using UnityEngine;

public interface IHealthBehavior
{    
    float MaxHealth { get; }
    float CurrentHealth { get; set; }
    float HealthPercentage { get; set; }
    Transform HealthObject { get; set; }
    event Action<float> OnHealthPercentageChanged;
}
