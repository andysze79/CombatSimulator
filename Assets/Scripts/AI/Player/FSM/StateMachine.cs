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
        
        public ReferenceKeeper referenceKeeper{ get; set; }
        public PlayerDataHolder playerData{ get; set; }
        public PlayerLogic playerLogic{ get; set; }

        protected void Start()
        {
            var states = GetComponents<State>();
            
            foreach (var state in states)
            {
                m_States.Add(state);
            }
            
            EnterState(m_InitialState.GetType());

            GetReference();
        }
        private void GetReference() {
            referenceKeeper = GetComponent<ReferenceKeeper>();
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
        }
    }
}
