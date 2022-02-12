using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarBehavior : MonoBehaviour
{
    [SerializeField]private Image m_Health;
    [SerializeField]private Image m_Buffer;
    [SerializeField]private float m_YOffset;
    [Min(.3f)][SerializeField]private float m_BufferSpeed = .5f;
    [SerializeField]private AnimationCurve m_Movement;
    public IHealthBehavior healthBehavior;
    Coroutine BufferProcess { get; set; }
    
    public void SetHealth(IHealthBehavior healthBehavior) {
        this.healthBehavior = healthBehavior;
        //print(healthBehavior.HealthObject.name);
        healthBehavior.OnHealthPercentageChanged += OnHealthChanged;
    }    
    private void OnHealthChanged(float percentage) {
        m_Health.fillAmount = healthBehavior.CurrentHealth / healthBehavior.MaxHealth;
        
        if (BufferProcess != null)
            StopCoroutine(BufferProcess);

        StartCoroutine(Buffering());
    }
    private IEnumerator Buffering() {
        var startTime = Time.time;
        var endTime = 1/m_BufferSpeed;
        var from = m_Buffer.fillAmount;

        while (Time.time - startTime < endTime ) {

            m_Buffer.fillAmount = Mathf.Lerp(from, m_Health.fillAmount, m_Movement.Evaluate(Time.time - startTime) / endTime);
            yield return null;
        }

        m_Buffer.fillAmount = m_Health.fillAmount;
        BufferProcess = null;
    }
    protected virtual void LateUpdate()
    {
        transform.position = CombateSimulator.GameManager.Instance.MainCamera.WorldToScreenPoint(
            healthBehavior.HealthObject.position + Vector3.up * m_YOffset);
    }
}
