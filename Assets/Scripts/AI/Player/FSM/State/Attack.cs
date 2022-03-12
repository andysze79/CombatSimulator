using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public class Attack : State
    {
        private bool startTransition;
        protected override void OnEnable()
        {
            stateMachine.playerLogic.ActivateCamera();
            stateMachine.playerLogic.ActivateMelee();
            stateMachine.playerLogic.ActivateLockOnTarget();

            stateMachine.playerLogic.OnAttackStateExitDel += CheckStateTransition;
        }
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            stateMachine.playerLogic.DeactivateLockOnTarget();

            stateMachine.playerLogic.OnAttackStateExitDel -= CheckStateTransition;            
        }
        protected override void Update()
        {
            stateMachine.playerLogic.CharacterFacingEnemy(stateMachine.playerData.FacingTargetSpeed);

            if (startTransition)
            {
                if (stateMachine.playerLogic.CheckGrounded() && stateMachine.playerData.CurrentAttackStyle == EnumHolder.AttackStyle.None)
                {
                    stateMachine.EnterState(typeof(Idle)); 
                    startTransition = false;
                }
                else if (!stateMachine.playerLogic.CheckGrounded())
                {
                    stateMachine.EnterState(typeof(Idle)); 
                    startTransition = false;
                }
            }
        }
        private void CheckStateTransition() {
            startTransition = true;
        }        
    }
}