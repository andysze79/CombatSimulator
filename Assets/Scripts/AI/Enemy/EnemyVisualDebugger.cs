using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualDebugger : MonoBehaviour
{
    public bool m_AttackObjCheckSphere;
    public bool m_EnableLookCone;
    public Color m_LookConeColor;
    public StateController EnemyStateController { get; private set; }
    public CombateSimulator.EnemyAI.EnemyData EnemyData { get; private set; }
    
    public void OnDrawGizmos()
    {
        if (EnemyStateController == null)
        {
            EnemyStateController = GetComponent<StateController>();
            EnemyData = GetComponent<CombateSimulator.EnemyAI.EnemyData>();            
        }
        if (m_AttackObjCheckSphere)
        {
            Gizmos.DrawWireSphere(EnemyStateController.eyes.position, EnemyData.m_LookSphereCastRadius);
            Gizmos.DrawWireSphere(EnemyStateController.eyes.position + EnemyStateController.eyes.forward * EnemyData.m_AttackRange, EnemyData.m_LookSphereCastRadius);
        }
        if (m_EnableLookCone)
        {
            RaycastHit hit;
            float angleX = EnemyData.m_LookConeAngleX;
            float angleY = EnemyData.m_LookConeAngleY;
            int precision = EnemyData.m_LookPrecision;

            for (int i = -(int)angleY; i < angleY; i += precision)
            {
                for (int j = -(int)angleX; j < angleX; j += precision)
                {
                    var pos = EnemyStateController.eyes.position;

                    var dir = Quaternion.AngleAxis(i, Vector3.up) * EnemyStateController.transform.forward;
                    dir = Quaternion.AngleAxis(j, EnemyStateController.transform.right) * dir;

                    if (Physics.Raycast(pos, dir, out hit, EnemyData.m_LookRange))
                        Debug.DrawLine(pos, hit.point, m_LookConeColor);
                    else
                        Debug.DrawLine(pos, pos + dir * EnemyData.m_LookRange, m_LookConeColor);
                }
            }
        }
    }
}
