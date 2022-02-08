using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Bindings;
[RequireComponent(typeof(RenderSettingsEnumCreater))]
public class RendersettingsSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct RendersettingsGroup {
        public string Name;
        //
        // Summary:
        //     The density of the exponential fog.
        public float fogDensity;
        //
        // Summary:
        //     The color of the fog.
        public Color fogColor;
        //
        // Summary:
        //     Fog mode to use.
        public FogMode fogMode;
        //
        // Summary:
        //     The starting distance of linear fog.
        public float fogStartDistance;
        //
        // Summary:
        //     The ending distance of linear fog.
        public float fogEndDistance;
        //
        // Summary:
        //     Is fog enabled?
        public bool fog;
        //
        // Summary:
        //     How much the light from the Ambient Source affects the Scene.
        public float ambientIntensity;
        //
        // Summary:
        //     The fade speed of all flares in the Scene.
        public float flareFadeSpeed;
    }
    public List<RendersettingsGroup> m_RenderSettingsValuesGroup = new List<RendersettingsGroup>();
    public int m_Index;
    public float m_TraisitionTime;
    public int Index { get; set; }
    public RenderSettingsEnum m_Target; 
    public RenderSettingsEnum Target { get; set; }
    
    Coroutine Process { get; set; }

    private void OnEnable()
    {
        Index = m_Index;
        ChangeRenderSettings(m_Target);
    }
    void Update()
    {
        if (Index != m_Index) {
            Index = m_Index;
            ChangeRenderSettings(Index);
        }
        if (Target != m_Target) {
            Target = m_Target;

            ChangeRenderSettings(Target);
        }
    }

    public void ChangeRenderSettings(string name) {
        foreach (var item in m_RenderSettingsValuesGroup)
        {
            if (item.Name == name) {
                RenderSettings.fog = item.fog;
            }
        }
    }
    public void ChangeRenderSettings(RenderSettingsEnum renderSettingsEnum)
    {
        var index = (int)renderSettingsEnum;
        RenderSettings.fog = m_RenderSettingsValuesGroup[index].fog;

        if (Process == null)
            Process = StartCoroutine(Transition(index, m_TraisitionTime));
        else {
            StopCoroutine(Process);
            Process = StartCoroutine(Transition(index, m_TraisitionTime));
        }
    }
    public void ChangeRenderSettings(int index)
    {
        RenderSettings.fog = m_RenderSettingsValuesGroup[index].fog;
    }
    private IEnumerator Transition(int targetIndex, float duration) {
        var startTime = Time.time;
        var endTime = duration;
        var startColFrom = RenderSettings.fogColor;
        var startColTo = m_RenderSettingsValuesGroup[targetIndex].fogColor;
        var startDistFrom = RenderSettings.fogStartDistance;
        var startDistTo = m_RenderSettingsValuesGroup[targetIndex].fogStartDistance;
        var endDistFrom = RenderSettings.fogEndDistance;
        var endDistTo = m_RenderSettingsValuesGroup[targetIndex].fogEndDistance;

        while (Time.time - startTime < endTime)
        {
            RenderSettings.fogColor = Color.Lerp(startColFrom, startColTo, (Time.time - startTime) / endTime);
            RenderSettings.fogStartDistance = Mathf.Lerp(startDistFrom, startDistTo, (Time.time - startTime) / endTime);
            RenderSettings.fogEndDistance = Mathf.Lerp(endDistFrom, endDistTo, (Time.time - startTime) / endTime);
            yield return null;
        }

        RenderSettings.fogStartDistance = startDistTo;
        RenderSettings.fogEndDistance = endDistTo;
    }   
}
