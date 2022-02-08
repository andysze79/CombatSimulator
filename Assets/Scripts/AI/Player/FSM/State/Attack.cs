using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public class Attack : State
    {
        protected override void OnEnable()
        {
            stateMachine.playerLogic.ActivateCamera();
            stateMachine.playerLogic.ActivateMelee();
            //stateMachine.playerLogic.ActivateMove();
            stateMachine.playerLogic.ActivateLockOnTarget();
        }
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            //stateMachine.playerLogic.DeactivateMove();
            stateMachine.playerLogic.DeactivateLockOnTarget();
        }
        protected override void Update()
        {
            stateMachine.playerLogic.CharacterFacingEnemy(stateMachine.playerData.FacingTargetSpeed);
            if (stateMachine.playerLogic.CheckGrounded() && stateMachine.playerData.CurrentAttackStyle == EnumHolder.AttackStyle.None)
            {
                stateMachine.EnterState(typeof(Idle));
            }
            else if(!stateMachine.playerLogic.CheckGrounded())
            { 
                stateMachine.EnterState(typeof(Idle));            
            }
        }
    }
}