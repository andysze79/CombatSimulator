using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CombatSimulator/ EnemiesLibrary")]
public class EnemiesLibrary : ScriptableObject
{
    public enum EnemyGenra { Mob, Boss}
    public enum EnemyClass{ Assassin }
    public enum EnemyAIType{ Guard, Chaser, Scanner }
    [System.Serializable]
    public struct EnemyData {
        public string ID;
        public EnemyGenra EnemyGenra;
        public EnemyClass EnemyClass;
        public EnemyAIType EnemyAIType;
        public GameObject Prefab;
    }
    
    public List<EnemyData> m_EnemiesLibrary = new List<EnemyData>();
    public GameObject GetEnemy(string id) {
        for (int i = 0; i < m_EnemiesLibrary.Count; i++)
        {
            if (m_EnemiesLibrary[i].ID.Contains(id)) {
                return m_EnemiesLibrary[i].Prefab;
            }
        }
        return null;
    }
    public GameObject GetEnemy(EnemyGenra genra, EnemyClass enemyClass, EnemyAIType aiType) {
        for (int i = 0; i < m_EnemiesLibrary.Count; i++)
        {
            if (m_EnemiesLibrary[i].EnemyGenra == genra &&
                m_EnemiesLibrary[i].EnemyClass == enemyClass &&
                m_EnemiesLibrary[i].EnemyAIType == aiType )
            {
                return m_EnemiesLibrary[i].Prefab;
            }
        }
        return null;
    }
    public GameObject GetEnemy(EnemySpawnersController.EnemiesWayPointsLibrary waypointSettings) {
        for (int i = 0; i < m_EnemiesLibrary.Count; i++)
        {
            if (m_EnemiesLibrary[i].EnemyGenra == waypointSettings.EnemyGenra &&
                m_EnemiesLibrary[i].EnemyClass == waypointSettings.EnemyClass &&
                m_EnemiesLibrary[i].EnemyAIType == waypointSettings.EnemyAIType )
            {
                return m_EnemiesLibrary[i].Prefab;
            }
        }
        return null;
    }
}
