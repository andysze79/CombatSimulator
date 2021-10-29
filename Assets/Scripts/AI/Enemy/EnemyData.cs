using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyData : MonoBehaviour
{
    public enum EnemyType { Chaser, Scanner}
    public EnemyType m_EnemyType = EnemyType.Chaser;
    [ShowIf("m_EnemyType", EnemyType.Scanner)][BoxGroup("Scanner Type Settings")]
    public float m_SearchingTurnSpeed = 5;
    [ShowIf("m_EnemyType", EnemyType.Scanner)][BoxGroup("Scanner Type Settings")]
    public float m_SearchingDuration = 5;
    [BoxGroup("Search Settings")]
    public float m_LookSphereCastRadius = 5;
    [BoxGroup("Search Settings")]
    public float m_OverlapseSphereRadius = 20;
    [BoxGroup("Search Settings")]
    public float m_LookRange = 5;
    [BoxGroup("Search Settings")]
    public float m_ChaseDistance = 10;
    [BoxGroup("Attack Settings")]
    public float m_AttackRange = 1;
    [BoxGroup("Attack Settings")]
    public float m_AttackCDDuration = 4;
    [BoxGroup("Attack Settings")]
    public float m_AttackRate = 1;
    [BoxGroup("Hit Settings")]
    public float m_HitStunDuration = 3;
    [FoldoutGroup("AnimationTrigger")]
    public string m_IdleName;
    [FoldoutGroup("AnimationTrigger")]
    public string m_RunName;
    [FoldoutGroup("AnimationTrigger")]
    public string m_AttackName;

    [ReadOnly]public bool AttackCD;// { get; set; }
}
