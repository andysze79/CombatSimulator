using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CombatSimulator/ GlobalVariables")]
public class GlobalVariables : ScriptableObject
{ 
    [SerializeField] private float m_DefenseMulCoe;
    [SerializeField] private float m_DefensePushBackDevideCoe;
    /// <summary>
    /// For checking enemy hit the wall.
    /// </summary>
    [SerializeField] private LayerMask m_WallLayer;
    [SerializeField] private float m_HitWallCheckingRayLength;
    public float DefenseMulCoe { get => m_DefenseMulCoe; }
    public float DefensePushBackDevideCoe { get => m_DefensePushBackDevideCoe; }
    public LayerMask WallLayer { get => m_WallLayer; }
    public float HitWallCheckingRayLength { get => m_HitWallCheckingRayLength; }
}
