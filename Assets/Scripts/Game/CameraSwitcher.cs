using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CameraSwitcher : MonoBehaviour
{
    public GameObject m_PlayerCamera;
    public List<GameObject> m_Cameras = new List<GameObject>();

    [HorizontalGroup("Split",.5f)]
    [Button]
    public void ActivatePlayerCam() {
        for (int i = 0; i < m_Cameras.Count; i++)
        {
            m_Cameras[i].SetActive(false);
        }
        m_PlayerCamera.SetActive(true);
    }

    [HorizontalGroup("Split", .5f)]
    [Button]
    public void ActivateFirstCam() {
        for (int i = 0; i < m_Cameras.Count; i++)
        {
            m_Cameras[i].SetActive(false);
        }
        m_Cameras[0].SetActive(true);
    }
    public void ActivateCam(string name)
    {
        for (int i = 0; i < m_Cameras.Count; i++)
        {
            if (m_Cameras[i].name == name) 
                m_Cameras[i].SetActive(true);
            else
                m_Cameras[i].SetActive(false);
        }
    }
}
