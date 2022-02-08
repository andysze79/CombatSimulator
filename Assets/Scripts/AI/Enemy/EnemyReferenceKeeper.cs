using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombateSimulator.EnemyAI;

public class EnemyReferenceKeeper : MonoBehaviour
{
    public EnemyData EnemyData { get; set; }
    public EnemyLogic EnemyLogic { get; set; }
    public StateController EnemyStateController { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }

    private void Awake()
    {
        GetRef();
    }
    private void GetRef()
    {
        AnimationPlayer= GetComponentInChildren<AnimationPlayer>();
        EnemyData = GetComponent<EnemyData>();
        EnemyLogic = GetComponent<EnemyLogic>();
        EnemyStateController = GetComponentInChildren<StateController>();
    }
}
