using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace CombateSimulator.EnemyAI
{
    public class EnemyLogic : MonoBehaviour, IDamagable, IHealthBehavior
    {
        public static event Action<IHealthBehavior> OnHealthAdded = delegate { };
        public static event Action<IHealthBehavior> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPercentageChanged;

        public UltEvent WhenReceiveDamage;
        public delegate void EnemyDelegate(Transform target);
        public EnemyDelegate OnReceiveDamage;
        public EnemyReferenceKeeper referenceKeeper;
        int currentAttackIndex;
        Coroutine PositionLerpingProcess { get; set; }

        public float MaxHealth { get { return referenceKeeper.EnemyData.m_MaxHealth; } }

        public float CurrentHealth { get; set; }
        public float HealthPercentage { get; set; }
        public Transform HealthObject { get; set; }

        private void Awake()
        {
            referenceKeeper = GetComponent<EnemyReferenceKeeper>();
        }
        private void Start()
        {
            HealthModule.SetUpHealth(this, transform);
            OnHealthAdded(this);
            HealthModule.SwitchHealthBar(this, false);

            AssignAnimationEvent();
            AssignTriggerEvent(true);
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
        
        void IDamagable.OnReceiveDamage(float damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker)
        {
            WhenReceiveDamage?.Invoke();
            OnReceiveDamage?.Invoke(attacker);

            //print("Receive damage: " + damageAmount);
            HealthModule.SwitchHealthBar(this, true);
            HealthModule.ChangeHealth(this, -damageAmount);
            OnHealthPercentageChanged(HealthPercentage);
            if (CurrentHealth <= 0) HealthModule.SwitchHealthBar(this, false);
            //print("Current Health: " + CurrentHealth);

            StartCoroutine(CombatCoroutines.PositionLerping(attacker, transform, pushBackDistance, duration, movement));
        }      

    }
}
