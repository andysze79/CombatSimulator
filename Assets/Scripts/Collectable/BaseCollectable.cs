using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TriggerBase))]
public abstract class BaseCollectable : MonoBehaviour
{
    [SerializeField] protected float m_PickUpDuration;    
    [SerializeField] protected AnimationCurve m_PickUpMovement;
    [SerializeField] protected Vector3 m_Offset;
    public event System.Action<Collider> WhenMoveToPlayer = delegate { };
    public event System.Action<Collider> OnCollectDel = delegate { };
    public TriggerBase Trigger;
    protected virtual void Awake() {
        Trigger = GetComponent<TriggerBase>();
        Trigger.TriggerEnter += OnCollect;

        OnCollectDel += MoveToPlayer;
        WhenMoveToPlayer += DestroyCollectable;
    }
    protected virtual void OnDisable() {
        Trigger.TriggerEnter -= OnCollect;

        OnCollectDel -= MoveToPlayer;
        WhenMoveToPlayer -= DestroyCollectable;
    }
    public virtual void OnCollect(Collider col) {
        Trigger.TriggerEnter -= OnCollect;
        OnCollectDel?.Invoke(col);
    }
    protected virtual void MoveToPlayer(Collider col)
    {
        StartCoroutine(MoveCoroutines.PositionLerping(
            transform, 
            col.transform,
            m_Offset, 
            m_PickUpDuration, 
            m_PickUpMovement, 
            WhenMoveToPlayer, 
            col));
    }
    protected void DestroyCollectable(Collider col)
    {
        Destroy(gameObject);
    }
}
