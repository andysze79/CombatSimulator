using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public abstract void Initialize (StateController controller);
    public abstract void Act (StateController controller);
}
