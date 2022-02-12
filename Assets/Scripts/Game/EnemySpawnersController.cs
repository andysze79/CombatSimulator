using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawnersController : MonoBehaviour
{
    [System.Serializable]
    public struct EnemiesWayPointsLibrary
    {
        [FoldoutGroup("Enemy")] public int Index;
        [FoldoutGroup("Enemy")] public EnemiesLibrary.EnemyGenra EnemyGenra;
        [FoldoutGroup("Enemy")] public EnemiesLibrary.EnemyClass EnemyClass;
        [FoldoutGroup("Enemy")] public EnemiesLibrary.EnemyAIType EnemyAIType;
        public bool SpawnAt1stWayPoint;
        [HideIf("SpawnAt1stWayPoint")]public Transform SpawnPoint;
        public List<Transform> EnemiesWayPoints;
    }    
    public List<EnemySpawner> m_EnemySpawners = new List<EnemySpawner>();
    public bool m_ShowGizmos;
    [ReadOnly]public List<StateController> m_SpawnedEnemies;
    private void OnEnable()
    {
        EnemySpawner.OnSpawn += SpawnEnemies;
    }
    private void OnDisable()
    {
        EnemySpawner.OnSpawn -= SpawnEnemies;
    }
    [Button]
    public void SearchEnemySpawners() {
        EnemySpawner[] spawners = GameObject.FindObjectsOfType<EnemySpawner>();
        m_EnemySpawners = new List<EnemySpawner>(spawners);
    }
    private void SpawnEnemies(EnemySpawner spawner) {
        EnemiesLibrary enemiesLibrary = CombateSimulator.GameManager.Instance.m_EnemiesLibrary;
        Transform spawnPoint;
        GameObject enemyPrefab;
        StateController clone;

        foreach (var item in spawner.m_SpawnerSettings)
        {
            spawnPoint = item.SpawnAt1stWayPoint? item.EnemiesWayPoints[0] : item.SpawnPoint;
            enemyPrefab = enemiesLibrary.GetEnemy(item);
            clone = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, transform).GetComponentInChildren<StateController>();
            AddEnemy(clone, item.EnemiesWayPoints); 
        }
    }
    public void AddEnemy(StateController controller, List<Transform> wayPoints)
    {
        m_SpawnedEnemies.Add(controller);
        controller.SetupAI(true, wayPoints);
    }
    public void RemoveEnemy(StateController controller)
    {
        if (m_SpawnedEnemies.Contains(controller))
            m_SpawnedEnemies.Remove(controller);
    }
    private void OnDrawGizmos()
    {
        if (!m_ShowGizmos) return;

        foreach (var spawner in m_EnemySpawners)
        {
            foreach (var settings in spawner.m_SpawnerSettings)
            {
                for (int i = 0; i < settings.EnemiesWayPoints.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(settings.EnemiesWayPoints[i].position, .1f);

                    if (settings.EnemiesWayPoints.Count == 1) continue;

                    var next = i + 1; 
                    
                    if (i + 1 >= settings.EnemiesWayPoints.Count)
                    {                     
                        next = 0; 
                    }

                    Gizmos.DrawLine(settings.EnemiesWayPoints[i].position, settings.EnemiesWayPoints[next].position);

                    var dir = (settings.EnemiesWayPoints[next].position - settings.EnemiesWayPoints[i].position).normalized;
                    var angle = 15;
                    dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
                    Gizmos.DrawLine(settings.EnemiesWayPoints[next].position, settings.EnemiesWayPoints[next].position - dir * 1);

                    dir = Quaternion.AngleAxis(-angle * 2, Vector3.up) * dir;
                    Gizmos.DrawLine(settings.EnemiesWayPoints[next].position, settings.EnemiesWayPoints[next].position - dir * 1);
                }
            }
        }    
    }
}
