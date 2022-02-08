using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator.PlayerFSM
{
    public abstract class State : MonoBehaviour
    {        
        public StateMachine stateMachine { get; set; }        
        protected void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
        }
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void Update();
        protected virtual void WhenReceiveJoystick1Input(float horizontal, float vertical)
        {
            
        }
        protected virtual void WhenReceiveJump()
        {
            
        }
        protected virtual void WhenReceiveFight1Up()
        {
            
        }
        protected virtual void WhenReceiveFight2Down()
        {
            
        }
        protected virtual void WhenReceiveFight2Up()
        {
            
        }
        protected void WhenGetDamage() {
            stateMachine.EnterState(typeof(Hit));
        }
    }
}
