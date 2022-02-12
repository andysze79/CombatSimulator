using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TriggerBase))]
public class EnemySpawner : MonoBehaviour
{
    public static event Action<EnemySpawner> OnSpawn = delegate { };
    public int m_SpawnerIndex;
    public List<EnemySpawnersController.EnemiesWayPointsLibrary> m_SpawnerSettings = new List<EnemySpawnersController.EnemiesWayPointsLibrary>();
    private TriggerBase TriggerBase { get; set; }
    private void OnEnable()
    {
        TriggerBase = GetComponent<TriggerBase>();
        TriggerBase.TriggerEnter += PlayerEnterBattleArea;
    }
    private void OnDisable()
    {
        TriggerBase.TriggerEnter -= PlayerEnterBattleArea;        
    }
    private void PlayerEnterBattleArea(Collider other) {
        OnSpawn(this);
    }
}
