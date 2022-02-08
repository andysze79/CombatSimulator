using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public delegate void EmptyDel();
    public delegate void ObjectDel(Transform target);
    public delegate void PlayerDel(PlayerLogic player);
    public static ObjectDel WhenLockOnTarget;
    public static PlayerDel WhenPlayerSpawned;
    public static EmptyDel WhenUnlockTarget;
    public static EmptyDel WhenReceiveDamage;
}
