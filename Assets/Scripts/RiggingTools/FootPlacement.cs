using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlacement : MonoBehaviour
{
    [System.Serializable]
    public struct FootData
    {
        public Transform foot;
        public Transform footIK;
    }
    public List<FootData> m_FootData = new List<FootData>();
    [Range(0,3)]public float m_ActiveDist = .5f;
    public Vector3 m_RayOffset;
    public LayerMask m_LayerMask;
    RaycastHit hitInfo;
    Vector3 footIKPos;
    private void Update()
    {
        for (int i = 0; i < m_FootData.Count; i++)
        {
            if (!CheckGroundInfo(m_FootData[i].foot)) continue;

            footIKPos = m_FootData[i].footIK.position;
            m_FootData[i].footIK.position = new Vector3(footIKPos.x, hitInfo.point.y, footIKPos.z);
        }
    }
    private bool CheckGroundInfo(Transform feet) {
        Ray ray = new Ray();        

        ray.origin = feet.position + m_RayOffset;
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, out hitInfo, m_ActiveDist, m_LayerMask))
        {
            return true;
        }
        else {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_FootData.Count; i++)
        {
            Gizmos.DrawLine(m_FootData[i].foot.position + m_RayOffset, m_FootData[i].foot.position + m_RayOffset + Vector3.down * m_ActiveDist);
        }
    }
}
