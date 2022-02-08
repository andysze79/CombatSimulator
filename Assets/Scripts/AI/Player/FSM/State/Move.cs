using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public class Move : State
    {
        protected override void OnEnable()
        {
            stateMachine.playerLogic.ActivateCamera();
            stateMachine.playerLogic.ActivateMelee();
            stateMachine.playerLogic.ActivateMove();
            stateMachine.playerLogic.ActivateLockOnTarget();

            UserControllerGetter.Instance.Joystick1InputDelegate += WhenReceiveJoystick1Input;
            UserControllerGetter.Instance.JumpDownDelegate += WhenReceiveJump;
        }
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            stateMachine.playerLogic.DeactivateMove();
            stateMachine.playerLogic.DeactivateLockOnTarget();

            UserControllerGetter.Instance.Joystick1InputDelegate -= WhenReceiveJoystick1Input;
            UserControllerGetter.Instance.JumpDownDelegate -= WhenReceiveJump;
        }
        protected override void Update()
        {

        }
        protected override void WhenReceiveJoystick1Input(float horizontal, float vertical)
        {
            if (Mathf.Abs(stateMachine.playerData.CharacterController.velocity.magnitude) == 0)
            {
                stateMachine.EnterState(typeof(Idle));
            }
        }
        protected override void WhenReceiveJump()
        {
            if (!stateMachine.playerLogic.CheckGrounded())
            {
                stateMachine.EnterState(typeof(Jump));
            }
        }
    }
}