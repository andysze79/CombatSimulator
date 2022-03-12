using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public class Jump : State
    {
        protected override void OnEnable()
        {
            stateMachine.playerLogic.ActivateCamera();
            stateMachine.playerLogic.ActivateMelee();
            stateMachine.playerLogic.ActivateMove();
            stateMachine.playerLogic.ActivateLockOnTarget();

            stateMachine.playerData.CharacterController.slopeLimit = 90;
            stateMachine.playerData.CharacterController.stepOffset = 0;

            UserControllerGetter.Instance.Joystick1InputDelegate += WhenReceiveJoystick1Input;
        }
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            stateMachine.playerLogic.DeactivateMove();
            stateMachine.playerLogic.DeactivateLockOnTarget();

            stateMachine.playerData.CharacterController.slopeLimit = stateMachine.playerLogic.CharactorControllerSlopeLimitDefault;
            stateMachine.playerData.CharacterController.stepOffset = stateMachine.playerLogic.CharactorControllerStepOffsetDefault;

            UserControllerGetter.Instance.Joystick1InputDelegate -= WhenReceiveJoystick1Input;
        }
        protected override void Update()
        {
            
        }
        protected override void WhenReceiveJoystick1Input(float horizontal, float vertical)
        {                
            if (stateMachine.playerLogic.CheckGrounded())            
            {
                if (Mathf.Abs(stateMachine.playerData.CharacterController.velocity.magnitude) == 0f)
                {
                    stateMachine.EnterState(typeof(Idle));
                }
                else
                { 
                    stateMachine.EnterState(typeof(Move));                    
                }
            }
        }

    }
}