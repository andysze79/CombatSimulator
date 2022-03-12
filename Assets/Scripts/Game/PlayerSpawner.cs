using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerSpawner : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera m_3rdPersonCam;
    public GameObject m_Player;
    public GameObject Player { get; private set; }
    public List<Transform> m_SpawnPoints = new List<Transform>();
    [Button]
    public void PositionPlayer() {
        var point = GetSpawnPoint(m_SpawnPoints[0].name);

        m_Player.transform.position = point.position;
        m_Player.transform.rotation = point.rotation;
    }
    private Transform GetSpawnPoint(string name) {
        for (int i = 0; i < m_SpawnPoints.Count; i++)
        {
            if (m_SpawnPoints[i].name == name) {
                return m_SpawnPoints[i];
            }        
        }
        return null;
    }
    private void Awake()
    {
        SetUpPlayer();
    }
    private void SetUpPlayer() {
        Player = m_Player;
        var playerData = Player.GetComponent<PlayerDataHolder>();
        m_3rdPersonCam.Follow = playerData.CinemachineCameraTarget.transform;
        playerData._3rdPersonCamera = Camera.main;
    }
}
