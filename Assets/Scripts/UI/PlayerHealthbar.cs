using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthbar : HealthbarBehavior
{
    private void OnEnable()
    {
        EventHandler.WhenPlayerSpawned += SetUpPlayer;
    }
    private void OnDisable()
    {
        EventHandler.WhenPlayerSpawned -= SetUpPlayer;        
    }
    private void SetUpPlayer(PlayerLogic player) {        
        SetHealth(player);
    }
    protected override void LateUpdate()
    {
        
    }
}
