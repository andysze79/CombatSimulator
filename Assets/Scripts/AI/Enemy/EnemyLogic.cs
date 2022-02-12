using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace CombateSimulator.EnemyAI
{
    public class EnemyLogic : MonoBehaviour, IDamagable, IHealthBehavior, IAnimationEvent
    {
        public static event Action<IHealthBehavior> OnHealthAdded = delegate { };
        public static event Action<IHealthBehavior> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPercentageChanged;

        public UltEvent WhenReceiveDamageWhileBlocking;
        public UltEvent WhenReceiveDamage;
        [SerializeField] private UltEvent WhenStartDefend;
        [SerializeField] private UltEvent WhenEndDefend;
        public delegate void EnemyDelegate(Transform target);
        public EnemyDelegate OnReceiveDamage;
        public EnemyReferenceKeeper referenceKeeper;
        int currentAttackIndex;
        Coroutine PositionLerpingProcess { get; set; }

        public Rigidbody rigidbody { get; set; }
        public float MaxHealth { get { return referenceKeeper.EnemyData.m_MaxHealth; } }
        public float CurrentHealth { get; set; }
        public float HealthPercentage { get; set; }
        public Transform HealthObject { get; set; }

        private void Awake()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();
            referenceKeeper = GetComponent<EnemyReferenceKeeper>();
        }
        private void Start()
        {
            HealthModule.SetUpHealth(this, transform);
            OnHealthAdded(this);
            HealthModule.SwitchHealthBar(this, false);

            AssignAnimationEvent();
            AssignTriggerEvent(true);
            
            referenceKeeper.AnimationPlayer.AnimatorRef.keepAnimatorControllerStateOnDisable = true;
            foreach (var smb in referenceKeeper.AnimationPlayer.AnimatorRef.GetBehaviours<AnimationEventSMB>())
            {
                smb.target = this;
            }
        }
        private void OnDisable()
        {
            ReleaseAnimationEvent();
            AssignTriggerEvent(false);
            OnHealthRemoved(this);
        }

        #region Animation Event
        private void AssignAnimationEvent (){
            referenceKeeper.AnimationPlayer.WhenTurnOnDamageTrigger += TurnOnDamageTrigger;
            referenceKeeper.AnimationPlayer.WhenTurnOffDamageTrigger += TurnOffDamageTrigger;
        }
        private void ReleaseAnimationEvent (){
            referenceKeeper.AnimationPlayer.WhenTurnOnDamageTrigger -= TurnOnDamageTrigger;
            referenceKeeper.AnimationPlayer.WhenTurnOffDamageTrigger -= TurnOffDamageTrigger;
        }
        private void TurnOnDamageTrigger(int Index)
        {
            currentAttackIndex = Index;
            referenceKeeper.EnemyData.m_AttackSettings[Index].AttackTrigger.gameObject.SetActive(true);
        }
        private void TurnOffDamageTrigger(int Index)
        {
            currentAttackIndex = 0;
            referenceKeeper.EnemyData.m_AttackSettings[Index].AttackTrigger.gameObject.SetActive(false);
        }
        #endregion
        #region Attack
        private void AssignTriggerEvent(bool AssignOrRelease) {
            DamageTrigger trigger;

            for (int i = 0; i < referenceKeeper.EnemyData.m_AttackSettings.Length; i++)
            {               
                trigger = referenceKeeper.EnemyData.m_AttackSettings[i].AttackTrigger.GetComponent<DamageTrigger>();

                if (AssignOrRelease)
                    trigger.TriggerEnter += DealDamage;
                else
                    trigger.TriggerEnter -= DealDamage;
            }
            
            if (referenceKeeper.EnemyData.m_HitVFXTrigger.TryGetComponent(out DamageTrigger HitTrigger))
            {
                HitTrigger.TriggerEnter += PlayHitVFX;
            }
        }
        private void PlayHitVFX(Collider target)
        {
            var cloneHitVFX = Instantiate(referenceKeeper.EnemyData.m_HitVFX, target.ClosestPoint(referenceKeeper.EnemyData.m_HitVFXTrigger.transform.position), Quaternion.identity, target.transform);
            Destroy(cloneHitVFX, 2f);
        }
        private void DealDamage(Collider target) {
            IDamagable damagableTarget = target.GetComponentInParent<IDamagable>();

            if (damagableTarget == null) return; 
            
            damagableTarget.OnReceiveDamage(
             referenceKeeper.EnemyData.m_AttackSettings[currentAttackIndex].DamageAmount,
             referenceKeeper.EnemyData.m_AttackSettings[currentAttackIndex].PushDistance,
             referenceKeeper.EnemyData.m_AttackSettings[currentAttackIndex].PushDuration,
             referenceKeeper.EnemyData.m_AttackSettings[currentAttackIndex].PushBackMovement,
             referenceKeeper.EnemyData.transform);
        }
        #endregion
        #region Stun
        public void CheckStun(bool value) {
            referenceKeeper.EnemyData.Stun = !value;
        }
        #endregion
        public void StartDefense() {
            referenceKeeper.EnemyData.isDefensing = true;
            referenceKeeper.AnimationPlayer.ChangeLayer(referenceKeeper.EnemyData.m_DefenseAnimatorLayerIndex, 1, referenceKeeper.EnemyData.m_DefenseSwitchLayerDuration);
            WhenStartDefend?.Invoke();
        }
        public void EndDefense()
        {            
            referenceKeeper.EnemyData.isDefensing = false;
            referenceKeeper.AnimationPlayer.ChangeLayer(referenceKeeper.EnemyData.m_DefenseAnimatorLayerIndex, 0, referenceKeeper.EnemyData.m_DefenseSwitchLayerDuration);
            WhenEndDefend?.Invoke();
        }
        public void CheckAttackSuccess(out bool success)
        {            
            success = referenceKeeper.EnemyData.isDefensing ? false : true;                
        }
        void IDamagable.OnReceiveDamage(float damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker)
        {
            if (referenceKeeper.EnemyData.Invulnerable) return; 
            if (!referenceKeeper.EnemyData.isDefensing) WhenReceiveDamage?.Invoke();
            if (referenceKeeper.EnemyData.isDefensing) WhenReceiveDamageWhileBlocking?.Invoke();

            OnReceiveDamage?.Invoke(attacker);

            DamageCalculation(damageAmount);

            if (referenceKeeper.EnemyData.isDefensing)
                pushBackDistance = pushBackDistance / CombateSimulator.GameManager.Instance.m_GlobalVariables.DefensePushBackDevideCoe;
                        
            StartCoroutine(CombatCoroutines.PositionLerping(attacker, transform, pushBackDistance, duration, movement));
        }
        private void DamageCalculation(float damageAmount) {
            
            if (referenceKeeper.EnemyData.isDefensing)
            { 
                damageAmount = damageAmount / (CombateSimulator.GameManager.Instance.m_GlobalVariables.DefenseMulCoe * referenceKeeper.EnemyData.m_DefenseAmount);
                damageAmount = Mathf.Clamp(damageAmount, 0, float.PositiveInfinity);
                //print(damageAmount);
            }

            //print("Receive damage: " + damageAmount);
            HealthModule.SwitchHealthBar(this, true);
            HealthModule.ChangeHealth(this, -damageAmount);
            OnHealthPercentageChanged(HealthPercentage);
            if (CurrentHealth <= 0) HealthModule.SwitchHealthBar(this, false);
            //print("Current Health: " + CurrentHealth);
        }

        #region SMB Events
        public void OnStateExit(int damageTriggerIndex)
        {
            TurnOffDamageTrigger(damageTriggerIndex);
        }
        #endregion
    }
}
