using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private bool m_FollowXAxis = true;
    [SerializeField] private bool m_FollowZAxis = true;
    [SerializeField] private bool m_FollowYAxis = false;
    public Vector3 m_AdditionalOffset;
    public bool m_HaveOffset = true;
    public Transform Target { get { return m_Target; } set { m_Target = value; } }
    public Vector3 offset { get; set; }
    public Vector3 PosPre { get; set; }

    public void OnEnable()
    {
        if(m_HaveOffset && m_Target)
            offset = transform.position - m_Target.position;
    }
    
    public void Update()
    {
        if (m_Target == null) return;

        if (m_Target.position != PosPre) {
            var moveXfer = transform.position;

            if(m_FollowXAxis)
                moveXfer.x = m_Target.position.x + offset.x;
            if(m_FollowZAxis)
                moveXfer.z = m_Target.position.z + offset.z;
            if(m_FollowYAxis)
                moveXfer.y = m_Target.position.y + offset.y;

            transform.position = moveXfer + m_AdditionalOffset;

            PosPre = transform.position;
        }
    }

}
