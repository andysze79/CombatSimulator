using CombateSimulator.EnemyAI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{

    public State currentState;
    public Transform eyes;
    [FoldoutGroup("States Settings")] public State remainState;
    [FoldoutGroup("States Settings")] public State HitState;
    [FoldoutGroup("States Settings")] public State DefenseHitState;
    [ReadOnly] [FoldoutGroup("Debug")] public State previousState;
    [ReadOnly] [FoldoutGroup("Debug")] public EnemyData enemyStats;
    [ReadOnly] [FoldoutGroup("Debug")] public EnemyLogic enemyLogic;
    [ReadOnly] [FoldoutGroup("Debug")] public float stateTimeElapsed;
    [ReadOnly] [FoldoutGroup("Debug")] public Transform chaseTarget;
    [SerializeField] private bool m_DebugMode = false;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    /*[HideInInspector] */
    public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;

    private bool aiActive;
    public float LookAroundStep { get; set; }
    public Vector3 rot { get; set; }
    public Vector3 LookAroundFrom { get; set; }
    public Vector3 LookAroundTo { get; set; }    
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
        if(enemyStats.m_PreSpawn)
            CombateSimulator.GameManager.Instance.EnemySpawnersController.AddEnemy(this, enemyStats.m_WayPoints);
        
        SetAnimationTrigger(currentState.playThisAnimation.ToString());

        enemyLogic.OnReceiveDamage += ReceiveDamage;
    }
    private void OnDisable()
    {
        CombateSimulator.GameManager.Instance.EnemySpawnersController.RemoveEnemy(this);        

        enemyLogic.OnReceiveDamage -= ReceiveDamage;
    }
    public void SetupAI(bool aiActivation, List<Transform> wayPointsFromTankManager)
    {
        wayPointList = wayPointsFromTankManager;
        aiActive = aiActivation;
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
            Gizmos.DrawWireSphere(eyes.position, .1f);
        }        
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            if(m_DebugMode) print(nextState);
            previousState = currentState;
            currentState = nextState;
            previousState.ExitState(this);
            currentState.EnterState(this);
            OnExitState();

            if(currentState.playThisAnimation != State.AnimationTriggerName.None)
            {
                ResetAnimationTrigger(previousState.playThisAnimation.ToString());
                SetAnimationTrigger(currentState.playThisAnimation.ToString());
            }
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {        
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }
    public WayPointInfo GetCurrentWayPointInfo() {
        wayPointList[nextWayPoint].TryGetComponent<WayPointInfo>(out WayPointInfo info);        
        return info;
    }
    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
    public bool CheckCurrentAnimationEnded(string animationName) {
        int layer = (animator.GetLayerWeight(enemyStats.m_DefenseAnimatorLayerIndex) > 0.5f) ? enemyStats.m_DefenseAnimatorLayerIndex : 0 ;
        bool isTargetAnimation = animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName);
        bool isTargetAnimationFinished = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= .95f;
        return isTargetAnimation && isTargetAnimationFinished;
    }
    public void SetAnimationTrigger(string name) {
        animator.SetTrigger(name);
    }
    private void ResetAnimationTrigger(string name)
    {
        animator.ResetTrigger(name);
    }
    private void ReceiveDamage(Transform target) {
        if (HitState == null) { Debug.Log(transform.name + " need Hit State"); return; }
        if (chaseTarget == null) { Debug.Log(target.name); chaseTarget = target; }

        if (!enemyStats.isDefensing)
        { 
            TransitionToState(HitState); 
        }
        if (enemyStats.isDefensing)
        {
            if (DefenseHitState == null) { Debug.Log(transform.name + " need DefenseHit State"); return; }
            TransitionToState(DefenseHitState);
        }
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
