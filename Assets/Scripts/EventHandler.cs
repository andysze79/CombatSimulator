using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public delegate void EmptyDel();
    public delegate void ObjectDel(Transform target);
    public static ObjectDel WhenLockOnTarget;
    public static EmptyDel WhenUnlockTarget;
}
