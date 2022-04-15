using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickUIMover : MonoBehaviour,IUIFloatTwo
{
    public Image m_Image;
    public float m_Radius;
    public bool m_InvertX = false;
    public bool m_InvertY = false;
    private Vector3 originalPos;
    public UltEvents.UltEvent WhenHasInput;
    public UltEvents.UltEvent WhenNoInput;
    private void Awake()
    {
        originalPos = m_Image.rectTransform.anchoredPosition;        
    }    
    private void MoveImage(float horizontal, float vertical) {
        if (Mathf.Abs(horizontal) < .1f && Mathf.Abs(vertical) < .1f) { 
            m_Image.rectTransform.anchoredPosition = originalPos;
            WhenNoInput.Invoke(); 
            return; 
        }

        if(m_InvertX) horizontal = -horizontal;
        if(m_InvertY) vertical = -vertical;
        var dir = new Vector3(horizontal, vertical, 0).normalized;
        m_Image.rectTransform.anchoredPosition = originalPos + dir * m_Radius;

        WhenHasInput.Invoke();
    }

    public void OnTriggered(float horizontal, float vertical)
    {
        MoveImage(horizontal, vertical);
    }
}
