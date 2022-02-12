using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CombatSimulator/ GlobalVariables")]
public class GlobalVariables : ScriptableObject
{ 
    [SerializeField] private float m_DefenseMulCoe;
    [SerializeField] private float m_DefensePushBackDevideCoe;
    public float DefenseMulCoe { get => m_DefenseMulCoe; }
    public float DefensePushBackDevideCoe { get => m_DefensePushBackDevideCoe; }
}
