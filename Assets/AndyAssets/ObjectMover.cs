using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class ObjectMover : MonoBehaviour
{
    public GameObject m_Obj;
    public Vector3 m_From;
    public Vector3 m_To;
    public float m_Duration;
    public bool m_Reverse;
    public bool m_UnscaleTime = false;
    public bool m_CanInterupt = true;
    public AnimationCurve m_Movement;
    public bool m_LocalPos = false;
    public bool m_ResetDefaultWhenTrigger = true;

    [Header("From offset to Default pos")]
    public bool m_MakeItMoveFromOffsetToDefault = false;
    public Vector3 m_FromOffset = Vector3.zero;
    [Header("From current to offset")]
    public bool m_UseFromCurrentToOffset = false;
    
    public bool m_TriggerOnEnable = true;
    public UltEvent WhenFinishedMovement;
    Coroutine Process { get; set; }

    private void Awake()
    {
        if (m_MakeItMoveFromOffsetToDefault)
        {
            m_From = transform.position + m_FromOffset;
            m_To = transform.position;
            m_Obj = gameObject;
        }
        if (m_UseFromCurrentToOffset) 
        {
            SetTargetValue();
            m_Obj = gameObject;
        }
    }
    private void SetTargetValue() {
        if (!m_LocalPos)
        {
            m_From = transform.position;
            m_To = transform.position + m_FromOffset;
        }
        else { 
            m_From = transform.localPosition;
            m_To = transform.localPosition + m_FromOffset;        
        }
    }
    private void OnEnable()
    {
        if(m_TriggerOnEnable)
            Trigger();
    }

    public void Trigger()
    {
        if (m_UseFromCurrentToOffset && m_ResetDefaultWhenTrigger) SetTargetValue();
        if (!m_UnscaleTime)
        {
            if (Process == null)
                Process = StartCoroutine(Transition());
            else if (m_CanInterupt)
            {
                StopCoroutine(Process);
                Process = StartCoroutine(Transition());
            }
        }
        else
        {
            if (Process == null)
                Process = StartCoroutine(UnscaleTransition());
            else if (m_CanInterupt)
            {
                StopCoroutine(Process);
                Process = StartCoroutine(UnscaleTransition());
            }
        }
    }

    private IEnumerator Transition()
    {
        m_Obj.SetActive(true);

        var startTime = Time.time;
        var endTime = m_Duration;

        while (Time.time - startTime < endTime)
        {
            if(!m_LocalPos)
                m_Obj.transform.position = Vector3.Lerp(m_From, m_To, m_Movement.Evaluate((Time.time - startTime) / endTime));
            else
                m_Obj.transform.localPosition = Vector3.Lerp(m_From, m_To, m_Movement.Evaluate((Time.time - startTime) / endTime));

            yield return null;
        }

        if(!m_LocalPos)
            m_Obj.transform.position = m_To;
        else
            m_Obj.transform.localPosition = m_To;

        if (m_Reverse)
        {
            startTime = Time.time;

            while (Time.time - startTime < endTime)
            {
                if (!m_LocalPos)
                    m_Obj.transform.position = Vector3.Lerp(m_To, m_From, m_Movement.Evaluate((Time.time - startTime) / endTime));
                else
                    m_Obj.transform.localPosition = Vector3.Lerp(m_To, m_From, m_Movement.Evaluate((Time.time - startTime) / endTime));

                yield return null;
            }

            if (!m_LocalPos)
                m_Obj.transform.position = m_From;
            else
                m_Obj.transform.localPosition = m_From;

        }
        WhenFinishedMovement?.Invoke();
        Process = null;
    }
    private IEnumerator UnscaleTransition()
    {
        m_Obj.SetActive(true);

        var startTime = Time.unscaledTime;
        var endTime = m_Duration;

        while (Time.unscaledTime - startTime < endTime)
        {
            m_Obj.transform.position = Vector3.Lerp(m_From, m_To, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        m_Obj.transform.position = m_To;


        if (m_Reverse)
        {
            startTime = Time.unscaledTime;

            while (Time.unscaledTime - startTime < endTime)
            {
                m_Obj.transform.position = Vector3.Lerp(m_To, m_From, (Time.unscaledTime - startTime) / endTime);
                yield return null;
            }

            m_Obj.transform.position = m_From;
        }
        WhenFinishedMovement?.Invoke();
        Process = null;
    }
}
