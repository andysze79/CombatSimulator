using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombateSimulator.EnemyAI;
using Sirenix.OdinInspector;

namespace CombateSimulator
{
    public class GameManager : MonoBehaviour
    {
        public GlobalVariables m_GlobalVariables;
        public EnemiesLibrary m_EnemiesLibrary;
        public VFXLibrary m_VFXLibrary;
        public MoreMountains.Feedbacks.MMFeedbacks m_ReceiveDamageFeedback;
        [ReadOnly]public EnemySpawnersController m_EnemySpawnersController;
        [ReadOnly]public GlobalVFXController m_GlobalVFXController;
        public EnemySpawnersController EnemySpawnersController { get {
                if (m_EnemySpawnersController == null)
                    m_EnemySpawnersController = GetComponent<EnemySpawnersController>();
                return m_EnemySpawnersController;
            } 
        }
        public GlobalVFXController GlobalVFXController
        {
            get
            {
                if (m_GlobalVFXController == null)
                    m_GlobalVFXController = GetComponent<GlobalVFXController>();
                return m_GlobalVFXController;
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
