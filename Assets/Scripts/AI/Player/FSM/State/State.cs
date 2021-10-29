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
        protected abstract void Trasnsition();
    }
}
