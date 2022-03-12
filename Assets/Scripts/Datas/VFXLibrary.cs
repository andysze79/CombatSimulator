using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CombatSimulator/ VFXLibrary")]
public class VFXLibrary : ScriptableObject
{
    [System.Serializable]
    public struct VFXData {
        public string ID;
        public GameObject VFX;
    }
    public List<VFXData> m_VFXDatas = new List<VFXData>();

    public GameObject GetVFX(string id) {
        for (int i = 0; i < m_VFXDatas.Count; i++)
        {
            if (m_VFXDatas[i].ID == id)
                return m_VFXDatas[i].VFX;
        }
        Debug.LogError("Couldn't find VFX: " + id);
        return null;
    }
}
