using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material[] m_SkyBox;
    public Color[] m_Color;
    public int m_Index;
    public bool m_SwapSkybox;
    public float m_TransitionTime = 1f;
    public int Index { get; set; }
    public void ChangeRenderSettings(int index) {
        StartCoroutine(Transition(index));
    }
    private IEnumerator Transition(int index) {
        var starTime = Time.time;
        var endTime = m_TransitionTime;
        var from = RenderSettings.skybox.GetColor("_Tint");
        Color col;

        while (Time.time - starTime < endTime)
        {
            col = Color.Lerp(from, m_Color[index], (Time.time - starTime) / endTime);
            RenderSettings.skybox.SetColor("_Tint", col);
            yield return null;
        }
        RenderSettings.skybox.SetColor("_Tint", m_Color[index]);
        
        if(m_SwapSkybox)
            RenderSettings.skybox = m_SkyBox[index];
    }
    private void Update()
    {
        if (Index != m_Index) {
            Index = m_Index;

            ChangeRenderSettings(Index);            
        }
    }
    private void OnDisable()
    {
        RenderSettings.skybox.SetColor("_Tint", m_Color[0]);

    }
}
