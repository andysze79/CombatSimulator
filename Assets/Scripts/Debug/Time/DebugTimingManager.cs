using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimingManager : MonoBehaviour
{
    public float[] m_TimeScale;
    public int m_CurrentIndex = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (m_CurrentIndex < m_TimeScale.Length - 1) {
                ++m_CurrentIndex;
                Time.timeScale = m_TimeScale[m_CurrentIndex];
                Debug.Log("Time Scale : " + Time.timeScale);
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (m_CurrentIndex > 0)
            {
                --m_CurrentIndex;
                Time.timeScale = m_TimeScale[m_CurrentIndex];
                Debug.Log("Time Scale : " + Time.timeScale);
            }
        }
       
    }
}
