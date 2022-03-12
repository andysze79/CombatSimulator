using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVFXController : MonoBehaviour
{
    public string m_HitShieldVFXID = "Spark";
    public VFXLibrary VFXLibrary { get; set; }
    private void Awake() { 
        VFXLibrary = CombateSimulator.GameManager.Instance.m_VFXLibrary; 
    }
    private void OnEnable()
    {
        EventHandler.WhenHitShield += PlayHitShieldVFX;
    }
    private void OnDisable()
    {
        EventHandler.WhenHitShield -= PlayHitShieldVFX;
    }
    public void PlayGlobalVFX(string id, Vector3 position, Quaternion rotation) {
        var clone = Instantiate(VFXLibrary.GetVFX(id), position, rotation, transform);
        Destroy(clone, clone.GetComponentInChildren<ParticleSystem>().main.duration * 1.1f);
    }
    private void PlayHitShieldVFX(Vector3 position, Quaternion rotation)
    {
        PlayGlobalVFX(m_HitShieldVFXID, position, rotation);
    }
}
