using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombateSimulator.EnemyAI;

namespace CombateSimulator
{
    public class GameManager : MonoBehaviour
    {
        public GlobalVariables m_GlobalVariables;
        public EnemiesLibrary m_EnemiesLibrary;
        public MoreMountains.Feedbacks.MMFeedbacks m_ReceiveDamageFeedback;
        public EnemySpawnersController m_EnemySpawnersController;
        public EnemySpawnersController EnemySpawnersController { get {
                if (m_EnemySpawnersController == null)
                    m_EnemySpawnersController = GetComponent<EnemySpawnersController>();
                return m_EnemySpawnersController;
            } 
        }
        public Camera MainCamera { get; set; }
        public static GameManager m_Instance;
        public static GameManager Instance { 
            get {
                if (m_Instance == null)
                    m_Instance = GameObject.FindObjectOfType<CombateSimulator.GameManager>();

                return m_Instance;
            } 
        }
        private void Awake()
        {
            MainCamera = Camera.main;
        }
        private void OnEnable()
        {
            EventHandler.WhenReceiveDamage += PlayReceiveDamageFeedback;
        }
        private void OnDisable()
        {
            EventHandler.WhenReceiveDamage -= PlayReceiveDamageFeedback;            
        }
        private void PlayReceiveDamageFeedback() {
            m_ReceiveDamageFeedback.PlayFeedbacks();
        }                
    }
}
