using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class VFXLifeTime : MonoBehaviour
{
    public float m_LifeTime;
    public bool m_Destroy;
    public UltEvent WhenEndOfLifeTime;

    public void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }

    public IEnumerator AutoDisable() {
        yield return new WaitForSeconds(m_LifeTime);
        WhenEndOfLifeTime?.Invoke();
        gameObject.SetActive(false);
        if(m_Destroy)
            Destroy(gameObject);
    }


}
