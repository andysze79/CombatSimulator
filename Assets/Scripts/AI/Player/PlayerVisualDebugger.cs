using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualDebugger : MonoBehaviour
{
    public bool m_ShowDashDistance;
    public Color m_DashDistanceColor;
    public PlayerDataHolder PlayerData { get; private set; }
        
    public void OnDrawGizmos()
    {
        if (PlayerData == null)
        {
            PlayerData = GetComponent<PlayerDataHolder>();
        }
        if (m_ShowDashDistance) {
            Gizmos.color = m_DashDistanceColor;
            var charController = PlayerData.CharacterController.transform;
            Gizmos.DrawLine(charController.position, charController.position + charController.forward * PlayerData.DashForce);
        }
    }
}
