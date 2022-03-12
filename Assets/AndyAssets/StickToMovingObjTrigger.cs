using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToMovingObjTrigger : TriggerBase
{
    public ObjectMover m_ObjectMover;
    public Transform m_MovingObj;
    private Transform originalParent;
    private void OnEnable() {
        TriggerEnter += StickToMovingObj;
        TriggerStay += Move;
        TriggerExit += Release;        
    }
    private void OnDisable()
    {
        TriggerEnter -= StickToMovingObj;
        TriggerStay -= Move;
        TriggerExit -= Release;                
    }
    private void StickToMovingObj(Collider col) {
        originalParent = col.transform.parent;
        col.transform.SetParent(m_MovingObj);
        m_ObjectMover.movingPlayer = col.transform.GetComponentInParent<CharacterController>();
    }
    private void Move(Collider col) {
    }
    private void Release(Collider col)
    {
        col.transform.SetParent(originalParent);
        m_ObjectMover.movingPlayer = null;
    }
}
