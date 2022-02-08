using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombateSimulator.EnemyAI;

namespace CombateSimulator
{
    public class GameManager : MonoBehaviour
    {
        [System.Serializable]
        public struct EnemiesWayPointsLibrary {
            public int Index;
            public List<Transform> EnemiesWayPoints;            
        }
        public List<EnemiesWayPointsLibrary> m_EnemiesWayPoints = new List<EnemiesWayPointsLibrary>();
        // Remove
        //public List<Transform> m_EnemiesWayPoints = new List<Transform>();
        public List<StateController> m_SpawnedEnemies;
        public MoreMountains.Feedbacks.MMFeedbacks m_ReceiveDamageFeedback;
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
        public void AddEnemy(StateController controller)
        {
            m_SpawnedEnemies.Add(controller);
            controller.SetupAI(true, m_EnemiesWayPoints[controller.enemyStats.m_WaypointsIndex].EnemiesWayPoints);
        }
        public void RemoveEnemy(StateController controller)
        {
            if(m_SpawnedEnemies.Contains(controller))
                m_SpawnedEnemies.Remove(controller);
        }
        
    }
}
