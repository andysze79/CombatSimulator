using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HealthbarController : SerializedMonoBehaviour
{
    [SerializeField]private HealthbarBehavior m_HealthbarPrefab;
    [SerializeField]private Transform m_Container;
    [SerializeField]private Dictionary<IHealthBehavior, HealthbarBehavior> HealthBars = new Dictionary<IHealthBehavior, HealthbarBehavior>();
    
    private void Awake()
    {
        CombateSimulator.EnemyAI.EnemyLogic.OnHealthAdded += AddHealthbar;
        CombateSimulator.EnemyAI.EnemyLogic.OnHealthRemoved += RemoveHealthbar;
    }
    private void OnDisable()
    {
        CombateSimulator.EnemyAI.EnemyLogic.OnHealthAdded -= AddHealthbar;
        CombateSimulator.EnemyAI.EnemyLogic.OnHealthRemoved -= RemoveHealthbar;        
    }
    private void AddHealthbar(IHealthBehavior healthBehavior) {
        var clone = Instantiate(m_HealthbarPrefab, m_Container);

        HealthBars.Add(healthBehavior,clone);
        clone.SetHealth(healthBehavior); 
    }
    private void RemoveHealthbar(IHealthBehavior healthBehavior) {
        if (HealthBars.ContainsKey(healthBehavior)) {
            Destroy(HealthBars[healthBehavior].gameObject);
            HealthBars.Remove(healthBehavior);
        }
    }
    public void SwitchHealthBar(IHealthBehavior healthBehavior, bool onOff)
    {
        if (!HealthBars.ContainsKey(healthBehavior)) return;

        HealthBars[healthBehavior].gameObject.SetActive(onOff);
    }
}
