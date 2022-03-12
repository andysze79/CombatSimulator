using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public abstract void StartAbility(ReferenceKeeper playerRef);
    public abstract void DoAbility(ReferenceKeeper playerRef);
    public abstract void EndAbility(ReferenceKeeper playerRef);    
}
