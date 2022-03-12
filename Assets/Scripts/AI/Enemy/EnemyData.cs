using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace CombateSimulator.EnemyAI
{
    public class EnemyData : MonoBehaviour
    {
        public enum EnemyType { Chaser, Scanner, Guard }
        public EnemyType m_EnemyType = EnemyType.Chaser;
        [BoxGroup("PreSpawn Settings")]
        public bool m_PreSpawn;
        [ShowIf("m_PreSpawn")] public List<Transform> m_WayPoints = new List<Transform>();
        [ShowIf("m_EnemyType", EnemyType.Guard)]
        [BoxGroup("Look Around Settings")]
        public float m_LookAroundSpeed = 0;
        [BoxGroup("Search Settings")]
        public float m_SearchingTurnSpeed = 5;
        [BoxGroup("Search Settings")]
        public float m_SearchingDuration = 5;
        [BoxGroup("Search Settings")]
        public float m_LookSphereCastRadius = 5;
        [BoxGroup("Search Settings")]
        public float m_OverlapseSphereRadius = 20;
        [BoxGroup("Search Settings")]
        public float m_LookConeAngleX = 30;
        [BoxGroup("Search Settings")]
        public float m_LookConeAngleY = 30;
        [BoxGroup("Search Settings")][Min(1)]
        public int m_LookPrecision = 5;
        [BoxGroup("Search Settings")]
        public float m_LookRange = 5;
        [BoxGroup("Search Settings")]
        public float m_ChaseDistance = 10;
        [BoxGroup("Attack Settings")]
        public bool m_CheckAttackPositionFromAnimationEvent = false;
        [BoxGroup("Attack Settings")]
        public float m_AttackRange = 1;
        [BoxGroup("Attack Settings")]
        public float m_FacingSpeed = 10;
        [BoxGroup("Attack Settings")]
        public float m_AttackCDDuration = 4;
        [BoxGroup("Attack Settings")]
        public float m_AttackRate = 1;
        [BoxGroup("Attack Settings")]
        public TriggerBase m_HitVFXTrigger;
        [BoxGroup("Attack Settings")]
        [System.Serializable]
        public struct AttackSettings
        {
            public string Name;
            public TriggerBase AttackTrigger;
            public int DamageAmount;
            public float PushDistance;
            public float PushDuration;
            public AnimationCurve PushBackMovement;
        }
        public AttackSettings[] m_AttackSettings;
        [BoxGroup("Hit Settings")]
        public float m_HitStunDuration = 3;
        [BoxGroup("Death Settings")]
        public float m_DelayToDeathDuration = 3;
        [BoxGroup("Defense Settings")]
        public float m_DefenseAmount;
        [BoxGroup("Defense Settings")]
        public int m_DefenseAnimatorLayerIndex;
        [BoxGroup("Defense Settings")]
        public float m_DefenseSwitchLayerDuration;
        [FoldoutGroup("Attack Assistance Settings")]
        public float m_AttackAssistRange;
        [FoldoutGroup("Attack Assistance Settings")]
        public float m_AssistDuration;
        [FoldoutGroup("Attack Assistance Settings")]
        public AnimationCurve m_AssistMovement;
        [BoxGroup("Status Settings")]
        public float m_MaxHealth = 100;
        [FoldoutGroup("AnimationTrigger")]
        public string m_IdleName;
        [FoldoutGroup("AnimationTrigger")]
        public string m_RunName;
        [FoldoutGroup("AnimationTrigger")]
        public string m_AttackName;
        [FoldoutGroup("VFX Settings")]
        public GameObject m_HitVFX;

        [ReadOnly] public bool AttackCD;// { get; set; }
        [ReadOnly] public bool Stun;// { get; set; }
        [ReadOnly] public bool Invulnerable;
        [ReadOnly] public bool isDefensing;

    }
}
