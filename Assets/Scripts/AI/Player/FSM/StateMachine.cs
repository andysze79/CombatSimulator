using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CombateSimulator.PlayerFSM
{
    public class StateMachine : MonoBehaviour
    {
        public State m_InitialState;
        [ReadOnly][SerializeField] protected List<State> m_States;
        [ReadOnly][SerializeField] protected State currentState;
        [ReadOnly][SerializeField] protected State previousState;

        private ReferenceKeeper m_ReferenceKeeper;
        public ReferenceKeeper referenceKeeper{ 
            get { 
                if (m_ReferenceKeeper == null)
                    m_ReferenceKeeper = GetComponent<ReferenceKeeper>(); 
                return m_ReferenceKeeper; } }
        public PlayerDataHolder playerData{ get; set; }
        public PlayerLogic playerLogic{ get; set; }

        protected void Awake()
        {
            GetReference();

            var states = GetComponents<State>();
            
            foreach (var state in states)
            {
                m_States.Add(state);
            }
            
            EnterState(m_InitialState.GetType());
        }
        protected void Update()
        {
            currentState?.DoAbility(referenceKeeper);
        }
        private void GetReference() {
            playerLogic = referenceKeeper.PlayerLogic;
            playerData = referenceKeeper.PlayerData;
        }
        public void EnterState(Type state) {
            for (int i = 0; i < m_States.Count; i++)
            {
                m_States[i].enabled = false;

                if (m_States[i].GetType() == state) {
                    previousState = currentState;
                    currentState = m_States[i];
                }
            }

            currentState.enabled = true;
            previousState?.EndAbility(referenceKeeper);
            currentState?.StartAbility(referenceKeeper);
        }
    }
}
