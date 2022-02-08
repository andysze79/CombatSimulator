using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointInfo : MonoBehaviour
{
    [SerializeField]private float[] m_LookAroundRange = new float[2];
    public float[] LookAroundRange { get { return m_LookAroundRange; } }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Gizmos.DrawLine(transform.position, 
            transform.position + Quaternion.Euler(0,m_LookAroundRange[0],0) * transform.forward * 2);

        Gizmos.DrawLine(transform.position,
            transform.position + Quaternion.Euler(0, m_LookAroundRange[1], 0) * transform.forward * 2);
    }
}
