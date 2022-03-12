using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CombateSimulator.PlayerFSM
{
    public class Hit : State
    {
        Coroutine process { get; set; }
        protected override void OnEnable()
        {
            if (process != null)
                StopCoroutine(process);
            process = StartCoroutine(HitStunDuration());

            stateMachine.playerLogic.CancelMovement();
        }
        protected override void OnDisable()
        {

        }
        
        protected override void Update()
        {
            stateMachine.playerLogic.ApplyGravity();
        }
        private IEnumerator HitStunDuration() {
            yield return new WaitForSeconds(stateMachine.playerData.StunDuration);

            stateMachine.EnterState(typeof(Idle));
        }
    }
}