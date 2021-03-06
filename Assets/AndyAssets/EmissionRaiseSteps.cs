using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EmissionRaiseSteps : EmissionRaise
{
    [ColorUsage(true, true)]
    [FoldoutGroup("Steps Setting"), Indent, PropertyOrder(9)]
    public Color[] m_ColTargets;
    [FoldoutGroup("Steps Setting"), Indent, PropertyOrder(9)]
    public float[] m_ValueTargets;
    [FoldoutGroup("Steps Setting"), Indent, PropertyOrder(9)]
    public int m_Index;

    public override void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
            switch (m_CurrentType)
            {
                case EmissionType.color:
                    RaiseToColor(m_Index);
                    break;
                case EmissionType.value:
                    RaiseToValue(m_Index);
                    break;
                default:
                    break;
            }
        }
    }

    public void RaiseToColor(int Index)
    {
        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, m_ColTargets[Index]);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, m_ColTargets[Index]);
    }
    public void RaiseToColor(Color color, string name, bool loop, bool flipflop, float duration)
    {
        StartCoroutine(ChangeDuration(EmissionType.color, name, loop, flipflop, duration));

        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, color);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, color);
    }
    public void RaiseToColor(int Index, bool loop, bool flipflop, float duration)
    {
        StartCoroutine(ChangeDuration(loop,flipflop, duration));

        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, m_ColTargets[Index]);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, m_ColTargets[Index]);
    }
    public void SetValue(int Index) {
        SetValue(m_TargetRenderer, m_ValueTargets[Index]);
    }
    public void SetColor(int Index)
    {
        SetColor(m_TargetRenderer, m_ColTargets[Index]);
    }
    public void RaiseToValue(int Index)
    {
        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, m_ValueTargets[Index]);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, m_ValueTargets[Index]);
    }
    public void RaiseToValue(float value, string name, bool loop, bool flipflop, float duration)
    {
        StartCoroutine(ChangeDuration(EmissionType.value, name, loop, flipflop, duration));

        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, value);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, value);
    }
    public void RaiseToValue(int Index, bool loop, bool flipflop, float duration)
    {
        StartCoroutine(ChangeDuration(loop, flipflop, duration));

        if (!m_UsingGroupRenderer)
            Raise(m_TargetRenderer, m_ValueTargets[Index]);
        else
            Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer, m_ValueTargets[Index]);
    }
    private IEnumerator ChangeDuration(bool loop, bool flipflop, float duration) {
        var loopOrigin = m_Loop;
        var flipflopOrigin = m_FlipFlop;
        var durationOrigin = m_Duration;

        m_Loop = loop;
        m_FlipFlop = flipflop;
        m_Duration = duration;
        yield return new WaitForSeconds(duration + .5f);
        //m_Loop = loopOrigin;
        //m_FlipFlop = flipflopOrigin;
        //m_Duration = durationOrigin;
    }
    private IEnumerator ChangeDuration(EmissionType Type, string name, bool loop, bool flipflop, float duration)
    {
        string nameOrigin;
        var loopOrigin = m_Loop;
        var flipflopOrigin = m_FlipFlop;
        var durationOrigin = m_Duration;

        switch (Type)
        {
            case EmissionType.color:
                nameOrigin = m_ControlColorName;
                m_ControlColorName= name;
                break;
            case EmissionType.value:
                nameOrigin = m_ControlValueName;
                m_ControlValueName = name;
                break;
            default:
                break;
        }
        
        m_Loop = loop;
        m_FlipFlop = flipflop;
        m_Duration = duration;
        yield return new WaitForSeconds(duration + .5f);
        //m_Loop = loopOrigin;
        //m_FlipFlop = flipflopOrigin;
        //m_Duration = durationOrigin;
    }
}
