using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointInfo : MonoBehaviour
{
    [Range(-360,360)][SerializeField]private float[] m_LookAroundRange = new float[2];
    [SerializeField]private bool m_ShowGizmos;
    public float[] LookAroundRange { 
        get {
            float[] range = { 0, 0};
            range[0] = m_LookAroundRange[0] + transform.eulerAngles.y;
            range[1] = m_LookAroundRange[1] + transform.eulerAngles.y;
            
            return range; 
        } 
    }
    public void OnDrawGizmos()
    {
        if (!m_ShowGizmos) return;

        Gizmos.color = Color.gray;

        Gizmos.DrawLine(transform.position, 
            transform.position + Quaternion.Euler(0,m_LookAroundRange[0],0) * transform.forward * 1);

        Gizmos.DrawLine(transform.position,
            transform.position + Quaternion.Euler(0, m_LookAroundRange[1], 0) * transform.forward * 1);
    }
}
