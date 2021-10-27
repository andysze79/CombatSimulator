using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceKeeper : MonoBehaviour
{
    public PlayerDataHolder PlayerData { get; set; }
    public PlayerLogic PlayerLogic { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }

    private void Awake()
    {
        GetRef();    
    }
    private void GetRef() {
        PlayerData = GetComponent<PlayerDataHolder>();
        PlayerLogic = GetComponent<PlayerLogic>();
        AnimationPlayer = GetComponentInChildren<AnimationPlayer>();
    }
}
