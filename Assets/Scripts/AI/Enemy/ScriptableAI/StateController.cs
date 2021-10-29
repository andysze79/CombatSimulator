using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class StateController : MonoBehaviour
{
    public State currentState;
    public Transform eyes;
    [FoldoutGroup("States Settings")] public State remainState;
    [FoldoutGroup("States Settings")] public State HitState;
    
    [ReadOnly] [FoldoutGroup("Debug")] public State previousState;
    [ReadOnly] [FoldoutGroup("Debug")] public EnemyData enemyStats;
    [ReadOnly] [FoldoutGroup("Debug")] public EnemyLogic enemyLogic;
    [ReadOnly] [FoldoutGroup("Debug")] public float stateTimeElapsed;
    [ReadOnly] [FoldoutGroup("Debug")] public Transform chaseTarget;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;

    private bool aiActive;

    Coroutine AttackCDProgress { get; set; }

    void Awake()
    {
        enemyStats = GetComponent<EnemyData>();
        enemyLogic = GetComponent<EnemyLogic>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        CombateSimulator.GameManager.Instance.AddEnemy(this);
        SetAnimationTrigger(currentState.playThisAnimation.ToString());

        enemyLogic.OnReceiveDamage += ReceiveDamage;
    }
    private void OnDisable()
    {
        CombateSimulator.GameManager.Instance.RemoveEnemy(this);        

        enemyLogic.OnReceiveDamage -= ReceiveDamage;
    }
    public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
    {
        wayPointList = wayPointsFromTankManager;
        aiActive = aiActivationFromTankManager;
        if (aiActive)
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }

    void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateState(this);
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, 1);// enemyStats.lookSphereCastRadius);
        }        
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            print(nextState);
            previousState = currentState;
            currentState = nextState;
            OnExitState();

            if(currentState.playThisAnimation != State.AnimationTriggerName.None)
                SetAnimationTrigger(currentState.playThisAnimation.ToString());
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {        
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
    public bool CheckCurrentAnimationEnded(string animationName) {
        bool isTargetAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        bool isTargetAnimationFinished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .95f;
        return isTargetAnimation && isTargetAnimationFinished;
    }
    private void SetAnimationTrigger(string name) {
        animator.SetTrigger(name);
    }
    private void ReceiveDamage() {
        if(HitState) TransitionToState(HitState);
    }
    public void StartAttackCD() {
        if (AttackCDProgress != null)
            StopCoroutine(AttackCDProgress);

        AttackCDProgress = StartCoroutine(StartAttackCDTimer());
    }
    private IEnumerator StartAttackCDTimer() {
        enemyStats.AttackCD = true;
        yield return new WaitForSeconds(enemyStats.m_AttackCDDuration);
        enemyStats.AttackCD = false;
    }
}
