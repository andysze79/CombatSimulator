using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public abstract class State : MonoBehaviour
    {        
        public StateMachine stateMachine { get; set; }
        public List<Ability> m_Abilities = new List<Ability>();
        protected void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
        }
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void Update();
        public void StartAbility(ReferenceKeeper playerRef) {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                m_Abilities[i].StartAbility(playerRef);
            }
        }
        public void DoAbility(ReferenceKeeper playerRef)
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                m_Abilities[i].DoAbility(playerRef);
            }
        }
        public void EndAbility(ReferenceKeeper playerRef)
        {
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                m_Abilities[i].EndAbility(playerRef);
            }
        }
        protected virtual void WhenReceiveJoystick1Input(float horizontal, float vertical)
        {
            
        }
        protected virtual void WhenReceiveJump()
        {
            
        }
        protected virtual void WhenReceiveFight1Up()
        {
            if (stateMachine.playerLogic.CheckGrounded() && stateMachine.playerData.CurrentAttackStyle != EnumHolder.AttackStyle.None)
            {
                stateMachine.EnterState(typeof(Attack));
            }
        }
        protected virtual void WhenReceiveFight2Down()
        {
            if (stateMachine.playerLogic.CheckGrounded() && stateMachine.playerData.CurrentAttackStyle != EnumHolder.AttackStyle.None)
            {
                stateMachine.EnterState(typeof(Attack));
            }
        }
        protected virtual void WhenReceiveFight2Up()
        {
            if (stateMachine.playerLogic.CheckGrounded() && stateMachine.playerData.CurrentAttackStyle != EnumHolder.AttackStyle.None)
            {
                stateMachine.EnterState(typeof(Attack));
            }
        }
        protected void WhenGetDamage() {
            stateMachine.EnterState(typeof(Hit));
        }
    }
}
