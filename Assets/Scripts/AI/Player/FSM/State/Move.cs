using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public class Move : State
    {
        protected override void OnDisable()
        {
            stateMachine.playerLogic.DeactivateCamera();
            stateMachine.playerLogic.DeactivateMelee();
            stateMachine.playerLogic.DeactivateMove();
            stateMachine.playerLogic.DeactivateLockOnTarget();
        }

        protected override void OnEnable()
        {
            stateMachine.playerLogic.ActivateCamera();
            stateMachine.playerLogic.ActivateMelee();
            stateMachine.playerLogic.ActivateMove();
            stateMachine.playerLogic.ActivateLockOnTarget();
        }

        protected override void Trasnsition()
        {
            throw new System.NotImplementedException();
        }

        protected override void Update()
        {
            if (stateMachine.playerData.CharacterController.velocity == Vector3.zero)
            {
                stateMachine.EnterState(typeof(Idle));
            }
        }
    }
}