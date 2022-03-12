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

            UserControllerGetter.Instance.Fight1UpDelegate += WhenReceiveFight1Up;
            UserControllerGetter.Instance.Fight2DownDelegate += WhenReceiveFight2Down;
            UserControllerGetter.Instance.Fight2UpDelegate += WhenReceiveFight2Up;
        }
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            stateMachine.playerLogic.DeactivateMove();
            stateMachine.playerLogic.DeactivateLockOnTarget();

            UserControllerGetter.Instance.Joystick1InputDelegate -= WhenReceiveJoystick1Input;
            UserControllerGetter.Instance.JumpDownDelegate -= WhenReceiveJump;

            UserControllerGetter.Instance.Fight1UpDelegate -= WhenReceiveFight1Up;
            UserControllerGetter.Instance.Fight2DownDelegate -= WhenReceiveFight2Down;
            UserControllerGetter.Instance.Fight2UpDelegate -= WhenReceiveFight2Up;
        }
        protected override void Update()
        {
            if (!stateMachine.playerLogic.CheckGrounded())
                stateMachine.EnterState(typeof(Jump));
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