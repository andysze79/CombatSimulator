using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition
{
    public Decision[] decision;
    public TransitionLogic logic;
    public State trueState;
    public State falseState;
}
public enum TransitionLogic
{
    Or, And
}
