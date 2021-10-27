using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class RootMotionScript : MonoBehaviour
{
    public Vector3 m_Direction;
    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            Vector3 newPosition = transform.position;
            if(m_Direction.x != 0)
                newPosition.x += animator.GetFloat("Runspeed") * Time.deltaTime * m_Direction.x;
            if (m_Direction.y != 0)
                newPosition.y += animator.GetFloat("Runspeed") * Time.deltaTime * m_Direction.y;
            if (m_Direction.z != 0)
                newPosition.z += animator.GetFloat("Runspeed") * Time.deltaTime * m_Direction.z;
            transform.position = newPosition;
        }
    }
}