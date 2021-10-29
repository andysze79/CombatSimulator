using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombateSimulator
{
    public class GameManager : MonoBehaviour
    {
        public List<Transform> m_EnemiesWayPoints = new List<Transform>();
        public List<StateController> m_SpawnedEnemies;
        public static GameManager m_Instance;
        public static GameManager Instance { 
            get {
                if (m_Instance == null)
                    m_Instance = GameObject.FindObjectOfType<CombateSimulator.GameManager>();

                return m_Instance;
            } 
        }

        public void AddEnemy(StateController controller)
        {
            m_SpawnedEnemies.Add(controller);
            controller.SetupAI(true, m_EnemiesWayPoints);
        }
        public void RemoveEnemy(StateController controller)
        {
            m_SpawnedEnemies.Remove(controller);
        }
        
    }
}
