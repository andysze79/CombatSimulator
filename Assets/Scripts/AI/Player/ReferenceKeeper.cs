using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceKeeper : MonoBehaviour
{
    private PlayerDataHolder m_PlayerData;
    private PlayerLogic m_PlayerLogic;
    public PlayerDataHolder PlayerData { get {
            if (m_PlayerData == null)
                m_PlayerData = GetComponent<PlayerDataHolder>();
            return m_PlayerData;
        }
    }
    public PlayerLogic PlayerLogic
    {
        get
        {
            if (m_PlayerLogic == null)
                m_PlayerLogic = GetComponent<PlayerLogic>();
            return m_PlayerLogic;
        }
    }
    public AnimationPlayer AnimationPlayer { get; set; }

    private void Awake()
    {
        GetRef();    
    }
    private void GetRef() {
        //PlayerData = GetComponent<PlayerDataHolder>();
        //PlayerLogic = GetComponent<PlayerLogic>();
        AnimationPlayer = GetComponentInChildren<AnimationPlayer>();
    }
}
