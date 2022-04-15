using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager m_Instance;
    public static UIManager Instance { 
        get 
        {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType<UIManager>();

            return m_Instance;
        }
    }
    
    public Image m_TargetRadical;
    public Image m_LockOnRadical;
    public float m_TargetRadicalYOffsetRate = 1.2f;
    public Camera MainCamera { get; set; }
    public HealthbarController HealthbarController { get; set; }
    Coroutine StickTargetSignProcess;
    private void OnEnable()
    {   
        MainCamera = Camera.main;
        HealthbarController = GetComponent<HealthbarController>();
        EventHandler.WhenAutoAimTarget += ShowTargetSign;
        EventHandler.WhenLockOnTarget += ShowLockOnSign;
        EventHandler.WhenNoTarget += HideTargetSign;
        EventHandler.WhenUnlockTarget += HideLockOnSign;
    }
    private void OnDisable()
    {
        EventHandler.WhenAutoAimTarget -= ShowTargetSign;
        EventHandler.WhenLockOnTarget -= ShowLockOnSign;        
        EventHandler.WhenNoTarget -= HideTargetSign;
        EventHandler.WhenUnlockTarget -= HideLockOnSign;        
    }
    private void ShowLockOnSign(Transform target) {
        m_LockOnRadical.gameObject.SetActive(true);
    }
    private void HideLockOnSign()
    {
        m_LockOnRadical.gameObject.SetActive(false);
    }
    private void ShowTargetSign(Transform target)
    {
        if (StickTargetSignProcess != null)
            StopCoroutine(StickTargetSignProcess);

        StickTargetSignProcess = StartCoroutine(StickTargetSign(target));
    }
    private void HideTargetSign() {
        if (StickTargetSignProcess != null)
            StopCoroutine(StickTargetSignProcess);

        m_TargetRadical.gameObject.SetActive(false);
    }
    private IEnumerator StickTargetSign(Transform target) {
        Vector3 screenPos;

        m_TargetRadical.gameObject.SetActive(true);

        while (target)
        {
            screenPos = MainCamera.WorldToScreenPoint(
                target.position + 
                new Vector3(0, target.GetComponentInChildren<CapsuleCollider>().height * m_TargetRadicalYOffsetRate, 0));

            m_TargetRadical.rectTransform.position = screenPos;
            yield return null;
        }

        m_TargetRadical.gameObject.SetActive(false);
    }
}
