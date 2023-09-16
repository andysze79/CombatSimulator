using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;

public class PlayerLogic : MonoBehaviour, IMatchTarget, IMatchSurface, IDamagable, IHealthBehavior, IAnimationEvent
{
    #region Variables
    public bool m_ConnectWithStateMachine;
    [ReadOnly] [SerializeField] private bool EnableCombo;
    [ReadOnly] [SerializeField] private bool ListenToMeleeAttack;
    public ReferenceKeeper referenceKeeper { get; set; }
    Coroutine InputCDProcess { get; set; }
    Coroutine AttackCDProcess { get; set; }
    Coroutine AnimationEventProcess { get; set; }
    public Vector3 TargetPosition
    {
        get
        {
            if (targetCollider)
                return targetCollider.ClosestPoint(transform.position);
            else
                return Vector3.positiveInfinity;
        }
    }

    public Vector3 TargetSurfacePosition {
        get; set;
    }

    public Collider targetCollider;

    [ReadOnly] [SerializeField] private float buttonDownTimer;
    [ReadOnly] [SerializeField] private bool isCharge = false;
    Coroutine checkButtonDownProcess;
    public Coroutine jumpProcess;
    EnumHolder.AttackStyle previousAttackStyle;
    EnumHolder.AttackStyle deriveToThisStyle;
    private float _rotationVelocity;
    private float _targetRotation;
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    public List<Collider> CurrentEnemies = new List<Collider>();
    public List<Collider> SortedTarget = new List<Collider>();

    [ReadOnly] public Vector3 moveValue;
    [ReadOnly] public Vector3 moveValueRaw;
    public delegate void StateMachineDel();
    public StateMachineDel WhenGetDamage;

    public event System.Action<float> OnHealthPercentageChanged;

    public System.Action OnAttackStateExitDel = delegate { };

    List<GameObject> AssignedTrigger = new List<GameObject>();
    Coroutine LockOnProcess { get; set; }
    private ParticleSystem CurrentAttackVFX { get; set; }
    public float MaxHealth { get { return referenceKeeper.PlayerData.MaxHealth; } }
    public float CurrentHealth { get; set; }
    public float HealthPercentage { get; set; }
    public Transform HealthObject { get; set; }
    private DamageTrigger HitTrigger { get; set; }
    public float CharactorControllerSlopeLimitDefault { get; private set; }
    public float CharactorControllerStepOffsetDefault { get; private set; }
    /// <summary>
    /// This boolean indicate whether the combe is ended and is not within the duration between attackended event and state exit
    /// </summary>
    private bool DurationFromAttackEndedToStateExitWhenComboEnded { get; set; }

    [ShowInInspector]private bool AttackSuccess = true;// { get; set; }
    private float currentMoveSpeed { get; set; }
    private bool isJumping { get; set; }
    public bool stopCheckGrounded { get; set; }
    #endregion
    private void OnEnable()
    {
        referenceKeeper = GetComponent<ReferenceKeeper>();        
        CharactorControllerSlopeLimitDefault = referenceKeeper.PlayerData.CharacterController.slopeLimit;
        CharactorControllerStepOffsetDefault = referenceKeeper.PlayerData.CharacterController.stepOffset;
        
        currentMoveSpeed = referenceKeeper.PlayerData.MoveSpeed;
    }

    private void Update()
    {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetBool(referenceKeeper.PlayerData.isGroundedName, CheckGrounded());
        referenceKeeper.AnimationPlayer.AnimatorRef.SetBool(referenceKeeper.PlayerData.isInAirName, CheckisInAir());
        
        if(!referenceKeeper.PlayerData.m_LockOn)
            SearchEnemyContinuously();
    }

    #region Assign Delegate
    private void Start()
    {
        HealthModule.SetUpHealth(this, referenceKeeper.PlayerData.CharacterController.transform);
        EventHandler.WhenPlayerSpawned.Invoke(referenceKeeper.PlayerLogic);

        #region Melee animation Delegate
        referenceKeeper.AnimationPlayer.WhenCheckComboStart += ComboCheckStart;
        referenceKeeper.AnimationPlayer.WhenCheckComboStart += SlowDownComboTime;
        referenceKeeper.AnimationPlayer.WhenCheckComboEnd += ComboCheckEnd;
        referenceKeeper.AnimationPlayer.WhenAttackEnded += AttackEnded;
        referenceKeeper.AnimationPlayer.WhenTurnOnDamageTrigger += TurnOnDamageTrigger;
        referenceKeeper.AnimationPlayer.WhenTurnOffDamageTrigger += TurnOffDamageTrigger;
        referenceKeeper.AnimationPlayer.WhenPlayComboVFX += PlayComboVFX;
        #endregion

        #region Assign Melee Trigger Event
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee1ComboTrigger, true);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee2ComboTrigger, true);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee3ComboTrigger, true);
        
        if (referenceKeeper.PlayerData.HitVFXTrigger.TryGetComponent(out DamageTrigger hitTrigger))
        {
            HitTrigger = hitTrigger;
            HitTrigger.TriggerEnter += PlayHitVFX;
        }
        #endregion


        // Animator Initialize
        referenceKeeper.AnimationPlayer.AnimatorRef.keepAnimatorControllerStateOnDisable = true;
        foreach (var smb in referenceKeeper.AnimationPlayer.AnimatorRef.GetBehaviours<MatchPositionSMB>())
        {
            smb.target = this;
        }

        foreach (var smb in referenceKeeper.AnimationPlayer.AnimatorRef.GetBehaviours<IdleParameterResetSMB>())
        {
            smb.m_PlayerDataHolder = referenceKeeper.PlayerData;
        }
        
        foreach (var smb in referenceKeeper.AnimationPlayer.AnimatorRef.GetBehaviours<MatchLandPositionSMB>())
        {
            smb.target = this;
        }

        foreach (var smb in referenceKeeper.AnimationPlayer.AnimatorRef.GetBehaviours<AnimationEventSMB>())
        {
            smb.target = this;
        }

        ActivateGuard();

        // Assign input delegate function
        if (m_ConnectWithStateMachine) return;
        ActivateMelee();
        ActivateCamera();
        ActivateMove();
        ActivateLockOnTarget();
    }
    private void OnDisable()
    {
        referenceKeeper.AnimationPlayer.WhenCheckComboStart -= ComboCheckStart;
        referenceKeeper.AnimationPlayer.WhenCheckComboStart -= SlowDownComboTime;
        referenceKeeper.AnimationPlayer.WhenCheckComboEnd -= ComboCheckEnd;
        referenceKeeper.AnimationPlayer.WhenAttackEnded -= AttackEnded;
        referenceKeeper.AnimationPlayer.WhenTurnOnDamageTrigger -= TurnOnDamageTrigger;
        referenceKeeper.AnimationPlayer.WhenTurnOffDamageTrigger -= TurnOffDamageTrigger;

        referenceKeeper.AnimationPlayer.WhenPlayComboVFX -= PlayComboVFX;

        #region Release Combo Trigger Event
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee1ComboTrigger, false);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee2ComboTrigger, false);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee3ComboTrigger, false);
        #endregion

        if (referenceKeeper.PlayerData.HitVFXTrigger.TryGetComponent(out DamageTrigger hitTrigger))
        {
            HitTrigger = hitTrigger;
            HitTrigger.TriggerEnter -= PlayHitVFX;
        }

        DeactivateGuard();

        if (m_ConnectWithStateMachine) return;
        DeactivateMelee();
        DeactivateCamera();
        DeactivateMove();
        DeactivateLockOnTarget();
    }
    private void AssignTriggerDelegate(GameObject[] meleeComboTrigger, bool AssignOrRelease) {
        DamageTrigger trigger;        

        for (int i = 0; i < meleeComboTrigger.Length; i++)
        {
            if (AssignedTrigger.Contains(meleeComboTrigger[i])) continue;

            trigger = meleeComboTrigger[i].GetComponent<DamageTrigger>();

            if (AssignOrRelease)
            { 
                trigger.TriggerEnter += CheckAttackEnemySuccess; 
                trigger.TriggerEnter += DealDamage; 
            }
            else
            { 
                trigger.TriggerEnter -= CheckAttackEnemySuccess; 
                trigger.TriggerEnter -= DealDamage; 
            }

            AssignedTrigger.Add(meleeComboTrigger[i]);
        }
    }
    #endregion
    #region Moving
    public int activateMoveCount;
    public void ActivateMove() {
        UserControllerGetter.Instance.Joystick1InputDelegate += CheckMove;
        ActivateRotate();    

        UserControllerGetter.Instance.JumpDownDelegate += CheckJump;
        UserControllerGetter.Instance.JumpUpDelegate += CheckFinishedJump;
        UserControllerGetter.Instance.DashDownDelegate += CheckDash;
        UserControllerGetter.Instance.RunDownDelegate += CheckRun;
        UserControllerGetter.Instance.RunUpDelegate += CheckFinishedRun;
    }
    public void DeactivateMove() {        
        UserControllerGetter.Instance.Joystick1InputDelegate -= CheckMove;
        DeactivateRotate();             

        UserControllerGetter.Instance.JumpDownDelegate -= CheckJump;
        UserControllerGetter.Instance.JumpUpDelegate -= CheckFinishedJump;
        UserControllerGetter.Instance.DashDownDelegate -= CheckDash; 
        UserControllerGetter.Instance.RunDownDelegate -= CheckRun;
        UserControllerGetter.Instance.RunUpDelegate -= CheckFinishedRun;
    }
    public void ActivateRotate() { activateMoveCount++; UserControllerGetter.Instance.Joystick1InputDelegate += CheckRotate; }
    public void DeactivateRotate() { activateMoveCount--; UserControllerGetter.Instance.Joystick1InputDelegate -= CheckRotate; }
    private void CheckMove(float horizontal, float vertical) {

        moveValue = new Vector3(horizontal, 0, vertical);

        var ChangeToCameraAlginment = Quaternion.AngleAxis(referenceKeeper.PlayerData._3rdPersonCamera.transform.eulerAngles.y, Vector3.up);
                
        moveValue = ChangeToCameraAlginment * moveValue;
        moveValueRaw = moveValue;
        SetAnimatorFloat(moveValue.x, moveValue.z);
        moveValue = moveValue * Time.deltaTime * currentMoveSpeed;

        // Gravity
        ApplyGravity();

        referenceKeeper.PlayerData.CharacterController.Move(moveValue);
    }
    public void CancelMovement()
    {
        moveValue = Vector3.zero;
    }
    public void ApplyGravity()
    {
        moveValue.y = referenceKeeper.PlayerData.Gravity * Time.deltaTime;
    }
    private void CheckRun() {
        currentMoveSpeed = referenceKeeper.PlayerData.RunSpeed;
        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.isRunningName, true);
    }
    private void CheckFinishedRun() { 
        currentMoveSpeed = referenceKeeper.PlayerData.MoveSpeed;        
        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.isRunningName, false);
    }
    private void CheckRotate(float horizontal, float vertical) {
        if (horizontal == 0 && vertical == 0) return;

        var moveDir = new Vector3(horizontal, 0, vertical);

        var ChangeToCameraAlginment = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        var targetDir = ChangeToCameraAlginment * moveDir;

        if (targetCollider && referenceKeeper.PlayerData.m_LockOn)
        { 
            targetDir = (targetCollider.transform.position - referenceKeeper.PlayerData.CharacterController.transform.position).normalized;
            targetDir.y = 0;
        }

        var newRot = Quaternion.LookRotation(targetDir);

        var ratio = Quaternion.Angle(referenceKeeper.PlayerData.CharacterController.transform.rotation, newRot);
        ratio = (ratio / 360) + 1;
        ratio *= 2;

        referenceKeeper.PlayerData.CharacterController.transform.rotation = Quaternion.RotateTowards(
            referenceKeeper.PlayerData.CharacterController.transform.rotation,
            newRot,
            Time.deltaTime * referenceKeeper.PlayerData.RotateSpeed * ratio);
    }
    
    public void CharacterFacingEnemy(float facingSpeed) {
        if (targetCollider == null) return;

        var lookDir = targetCollider.transform.position - referenceKeeper.PlayerData.CharacterController.transform.position;
        lookDir.y = 0;
        lookDir = lookDir.normalized;
        
        var newDir = Quaternion.LookRotation(lookDir); 
        referenceKeeper.PlayerData.CharacterController.transform.rotation = Quaternion.Lerp(
            referenceKeeper.PlayerData.CharacterController.transform.rotation, 
            newDir,
            Time.deltaTime * facingSpeed); 
    }

    #region Jump
    public bool CheckGrounded() {
        if(stopCheckGrounded) return false;

        RaycastHit[] hitInfo = new RaycastHit[3];

        int colAmount = Physics.SphereCastNonAlloc(
        referenceKeeper.PlayerData.CharacterController.transform.position + Vector3.up * referenceKeeper.PlayerData.CheckGroundedOffset,
        referenceKeeper.PlayerData.CheckGroundedSphereRadius,
        Vector3.down,
        hitInfo,
        referenceKeeper.PlayerData.CheckGroundedOffset + referenceKeeper.PlayerData.CheckGroundedDist,
        referenceKeeper.PlayerData.CheckGroundedLayer);
                
        if(colAmount != 0) return CheckSliding(hitInfo);

        return false;
    }
    private bool CheckSliding(RaycastHit[] hitInfo) {
        for (int i = 0; i < hitInfo.Length; i++)
        {
            if (hitInfo[i].collider == null) continue;

            //print(hitInfo[i].collider.name + " " + Vector3.Angle(Vector3.up, hitInfo[i].normal));
                                  
            if (Vector3.Angle(Vector3.up, hitInfo[i].normal) >= 45)
            {
                var speed = referenceKeeper.PlayerData.Gravity;
                float dot = Vector3.Dot(moveValue,hitInfo[i].normal);
                moveValue = (moveValue - hitInfo[i].normal * dot).normalized * speed;
                return false; 
            }
        }
        return true;
    }
    public bool CheckisInAir()
    {
        Vector3 pos= referenceKeeper.PlayerData.CharacterController.transform.position;
        Ray ray = new Ray(pos, -referenceKeeper.PlayerData.transform.up);
        RaycastHit hitInfo = new RaycastHit();
        bool result = Physics.Raycast(ray, out hitInfo, float.PositiveInfinity,referenceKeeper.PlayerData.CheckGroundedLayer);

        if (!result) return true;

        return Vector3.Distance(hitInfo.point, pos) >= referenceKeeper.PlayerData.isInAirDistance; 
    }
    public bool ignoreCheckGrounded = true;
    private void CheckJump()
    {
        if (!CheckGrounded() || referenceKeeper.PlayerData.JumpCD) return;

        Jump(referenceKeeper.PlayerData.JumpHight, ignoreCheckGrounded);
    }
    public void Jump(float jumpHight, bool ignoreCheckGrounded) {
        isJumping = true;

        if (jumpProcess != null)
            StopCoroutine(jumpProcess);

        jumpProcess = StartCoroutine(JumpProcess(jumpHight, ignoreCheckGrounded));

        SetAnimatorTrigger(referenceKeeper.PlayerData.JumpName);
    }
    private void CheckFinishedJump() {
        isJumping = false;
    }
    public Vector3 PlayerSpeed;
    public Vector3 JumpSpeed;
    private IEnumerator JumpProcess(float jumpHight, bool ignoreCheckGrounded)
    {        
        var startTime = Time.time;
        // v0 ^ 2 = -2gh 
        var velocity = Mathf.Sqrt(-2f * referenceKeeper.PlayerData.Gravity * referenceKeeper.PlayerData.JumpGravityMultiplier * jumpHight);
        var currentVelocity = referenceKeeper.PlayerData.CharacterController.velocity;
        while (true)
        {            
            velocity += referenceKeeper.PlayerData.Gravity * referenceKeeper.PlayerData.JumpGravityMultiplier * Time.deltaTime;
            referenceKeeper.PlayerData.CharacterController.Move(new Vector3(0, velocity, 0) * Time.deltaTime);
            JumpSpeed.y = velocity;
            PlayerSpeed = referenceKeeper.PlayerData.CharacterController.velocity;

            if (!ignoreCheckGrounded)
            {
                if (startTime > .5f && CheckGrounded()) break;
            }
            
            yield return null;
        }

        // Landing
        Landing();

        StartCoroutine(JumpCoolDown());
    }
    private void Landing() {
        RaycastHit HitInfo;
        Physics.Raycast(referenceKeeper.PlayerData.CharacterController.transform.position, -referenceKeeper.transform.up, out HitInfo, 100, referenceKeeper.PlayerData.CheckGroundedLayer);
        TargetSurfacePosition = HitInfo.point; 
    }
    #endregion

    #region Dash    
    private void CheckDash()
    {
        if (referenceKeeper.PlayerData.DashCD || !CheckGrounded()) return;

        SetAnimatorTrigger(referenceKeeper.PlayerData.DashName);

        referenceKeeper.PlayerData.DashVFX.Play(true);
        StartCoroutine(Dashing());
        StartCoroutine(DashCoolDown());
    }
    private IEnumerator Dashing() {
        var startTime = Time.time;
        var endTime = referenceKeeper.PlayerData.DashDuration;

        while (Time.time - startTime < endTime) {
            referenceKeeper.PlayerData.CharacterController.Move(
            referenceKeeper.PlayerData.CharacterController.transform.forward * referenceKeeper.PlayerData.DashForce * Time.deltaTime);
            yield return null;
        }
    }
    #endregion

    #endregion
    #region Guard
    public void ActivateGuard()
    {
        UserControllerGetter.Instance.GuardDownDelegate += StartGuard;
        UserControllerGetter.Instance.GuardUpDelegate += EndGuard;
    }
    public void DeactivateGuard()
    {
        UserControllerGetter.Instance.GuardDownDelegate -= StartGuard;
        UserControllerGetter.Instance.GuardUpDelegate -= EndGuard;
    }
    public void StartGuard() { referenceKeeper.AnimationPlayer.SetFloat(referenceKeeper.PlayerData.GuardName, 1, referenceKeeper.PlayerData.GuardTrasitionDuration); }
    public void EndGuard() { referenceKeeper.AnimationPlayer.SetFloat(referenceKeeper.PlayerData.GuardName, 0, referenceKeeper.PlayerData.GuardTrasitionDuration); }
    #endregion
    #region Melee
    public void ActivateMelee() {
        UserControllerGetter.Instance.Fight1DownDelegate += Melee1Attack;
        UserControllerGetter.Instance.Fight2DownDelegate += Melee2ButtonDown;
        UserControllerGetter.Instance.Fight2UpDelegate += Melee2Attack;
    }
    public void DeactivateMelee()
    {
        UserControllerGetter.Instance.Fight1DownDelegate -= Melee1Attack;
        UserControllerGetter.Instance.Fight2DownDelegate -= Melee2ButtonDown;
        UserControllerGetter.Instance.Fight2UpDelegate -= Melee2Attack;
    }    
    public void Melee1Attack() 
    {
        TriggerAttack(EnumHolder.AttackStyle.Melee1, referenceKeeper.PlayerData.Melee1Name, referenceKeeper.PlayerData.Melee1LastCombo);
    }
    public void Melee2ButtonDown() 
    {
        if (checkButtonDownProcess != null)
            StopCoroutine(checkButtonDownProcess);
                
        if(referenceKeeper.PlayerData.CurrentAttackStyle == EnumHolder.AttackStyle.None)
            checkButtonDownProcess = StartCoroutine(CheckButtonDownTimer(EnumHolder.AttackStyle.Melee2, EnumHolder.AttackStyle.Melee3, referenceKeeper.PlayerData.Melee3Name));
    }
    public void Melee2Attack()
    {
        //TriggerAttack(EnumHolder.AttackStyle.Melee2, referenceKeeper.PlayerData.Melee2Name, referenceKeeper.PlayerData.Melee2LastCombo);

        if (checkButtonDownProcess != null)
        { 
            StopCoroutine(checkButtonDownProcess);
        }

        if (!isCharge)
        {
            // For not fully charged melee2
            // ChangeAttackStyle(previousAttackStyle);
            TriggerAttack(EnumHolder.AttackStyle.Melee2, referenceKeeper.PlayerData.Melee2Name, referenceKeeper.PlayerData.Melee2LastCombo);
        }
        // Charge Attack
        else
        {
            isCharge = false;
            referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.ChargeName, false);

            referenceKeeper.PlayerData.m_WhenReleaseCharge?.Invoke();
        }
    }
    private bool CheckDeriveToOtherAttackStyle(EnumHolder.AttackStyle targetStyle, bool readyToSwitchStyle) {
        var result = false;

        var from = referenceKeeper.PlayerData.CurrentAttackStyle;
        var to = targetStyle;

        // Melee1 to Melee2
        if (from == EnumHolder.AttackStyle.Melee1 && to == EnumHolder.AttackStyle.Melee2)
        {
            if (referenceKeeper.PlayerData.CurrentCombo == referenceKeeper.PlayerData.Melee1ToMelee2[0])
            { 
                result = true;

                if (readyToSwitchStyle)
                {
                    referenceKeeper.PlayerData.CurrentCombo = referenceKeeper.PlayerData.Melee1ToMelee2[1];
                    ChangeAttackStyle(EnumHolder.AttackStyle.Melee2);
                    referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo1Name);
                    //print("from 1 to 2");
                }
                else { 
                    deriveToThisStyle = EnumHolder.AttackStyle.Melee2;                    
                }
            }
        }

        // Melee2 to Melee1
        if (from == EnumHolder.AttackStyle.Melee2 && to == EnumHolder.AttackStyle.Melee1)
        {
            if (referenceKeeper.PlayerData.CurrentCombo == referenceKeeper.PlayerData.Melee2ToMelee1[0])
            {
                result = true;

                if (readyToSwitchStyle)
                {
                    referenceKeeper.PlayerData.CurrentCombo = referenceKeeper.PlayerData.Melee2ToMelee1[1];
                    ChangeAttackStyle(EnumHolder.AttackStyle.Melee1);
                    referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo2Name);
                }
                else { 
                    deriveToThisStyle = EnumHolder.AttackStyle.Melee1;
                }
            }
        }

        if (result) 
        {
            //print("Derive " + referenceKeeper.PlayerData.CurrentCombo);
        }

        return result;
    }    
    private void ComboHandling() {
        // Check if reach to the last Combo
        if (referenceKeeper.PlayerData.CurrentCombo <= GetCurrentLastCombo())
        {
            ListenToMeleeAttack = false;
            EnableCombo = true;
        }
    }
    private EnumHolder.ComboCounter GetCurrentLastCombo() {
        switch (referenceKeeper.PlayerData.CurrentAttackStyle)
        {
            case EnumHolder.AttackStyle.None:
                return referenceKeeper.PlayerData.Melee1LastCombo;
            case EnumHolder.AttackStyle.Melee1:
                return referenceKeeper.PlayerData.Melee1LastCombo;
            case EnumHolder.AttackStyle.Melee2:
                return referenceKeeper.PlayerData.Melee2LastCombo;
            case EnumHolder.AttackStyle.Melee3:
                return referenceKeeper.PlayerData.Melee3LastCombo;
            default:
                return referenceKeeper.PlayerData.Melee1LastCombo;
        }
    }
    private void ChangeAttackStyle(EnumHolder.AttackStyle target) {
        previousAttackStyle = referenceKeeper.PlayerData.CurrentAttackStyle;
        referenceKeeper.PlayerData.CurrentAttackStyle = target;
    }
    private void SearchEnemyContinuously() {
        var result = SearchTarget();
        if (result) ChangeTargetCollider(result);
        else EventHandler.WhenNoTarget?.Invoke();
    }
    public void TriggerAttack(EnumHolder.AttackStyle attackStyle, string AnimatorTriggerName, EnumHolder.ComboCounter lastCombo) {

        // Check Derive to other Attack Style
        if (referenceKeeper.PlayerData.CurrentAttackStyle != EnumHolder.AttackStyle.None &&
            attackStyle != referenceKeeper.PlayerData.CurrentAttackStyle && 
            !CheckDeriveToOtherAttackStyle(attackStyle, false)) return;

        if (referenceKeeper.PlayerData.AttackCD) return;
        if (referenceKeeper.PlayerData.InputCD) return;
        if (referenceKeeper.PlayerData.CurrentCombo == lastCombo) return;
        
        InputCDProcess = StartCoroutine(InputCoolDown());


        // When start a new combo
        if (referenceKeeper.PlayerData.CurrentCombo == EnumHolder.ComboCounter.Combo1 &&
            !DurationFromAttackEndedToStateExitWhenComboEnded)
        { 
            ChangeAttackStyle(attackStyle); 
        }

        // When outside the combo check range
        if (!ListenToMeleeAttack)
        {
            referenceKeeper.AnimationPlayer.PlayAnimation(AnimatorTriggerName);
        }
        // When within the combo check range
        else
        {            
            if (referenceKeeper.PlayerData.EnableSlowMoWhenCheckTheseCombo.Length != 0) ResetTimeScale();
            ComboHandling();
        }
    }
    public IEnumerator CheckButtonDownTimer(EnumHolder.AttackStyle notChargedAttackStyle, EnumHolder.AttackStyle ChargedAttackStyle, string attackStyleAnimatorName) {
        var startTime = Time.time;
        var startChargeTime = referenceKeeper.PlayerData.StartChargeTime;
        var endChargeTime = referenceKeeper.PlayerData.ChargeTime;
        bool enteredCharge = false;

        while (Time.time - startTime < endChargeTime)
        {
            if (Time.time - startTime > startChargeTime && !enteredCharge)
            {
                enteredCharge = true;

                ChangeAttackStyle(notChargedAttackStyle);
                ChangeAttackStyle(ChargedAttackStyle);
                referenceKeeper.AnimationPlayer.PlayAnimation(attackStyleAnimatorName);

                referenceKeeper.PlayerData.m_WhenStartCharge?.Invoke();
            }

            buttonDownTimer = Time.time - startTime;            
            yield return null;
        }

        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.ChargeName, true);
        
        referenceKeeper.PlayerData.m_WhenFinsihedCharge?.Invoke();

        isCharge = true;
    }
    private void ResetComboCheckStatus()
    {
        referenceKeeper.PlayerData.CurrentCombo = 0;
        ChangeAttackStyle(EnumHolder.AttackStyle.None);
        AttackCDProcess = StartCoroutine(AttackCoolDown());

        DurationFromAttackEndedToStateExitWhenComboEnded = true;
    }
    #endregion
    #region Camera
    public void ActivateCamera() {
        UserControllerGetter.Instance.MouseInputDelegate += CameraRotation;
        //UserControllerGetter.Instance.MouseInputDelegate += CheckCameraMovement;
    }
    public void DeactivateCamera()
    {
        UserControllerGetter.Instance.MouseInputDelegate -= CameraRotation;
        //UserControllerGetter.Instance.MouseInputDelegate -= CheckCameraMovement;
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void CameraRotation(float horizontal, float vertical)
    {
        if (referenceKeeper.PlayerData.m_LockOn) return;

        Vector2 input = new Vector2(horizontal, vertical);

        // if there is an input and camera position is not fixed
        if (input.sqrMagnitude >= _threshold && !referenceKeeper.PlayerData.LockCameraPosition)
        {
            _cinemachineTargetYaw += input.x * Time.deltaTime * 100;
            _cinemachineTargetPitch += input.y * Time.deltaTime * 100;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, referenceKeeper.PlayerData.BottomClamp, referenceKeeper.PlayerData.TopClamp);

        // Cinemachine will follow this target
        referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + referenceKeeper.PlayerData.CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }
    private void CheckCameraMovement(float horizontal, float vertical) {
        // normalise input direction
        Vector3 inputDirection = new Vector3(horizontal, 0.0f, vertical).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (horizontal != 0 || vertical != 0)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + referenceKeeper.PlayerData._3rdPersonCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(
                referenceKeeper.PlayerData.CharacterController.transform.eulerAngles.y,
                _targetRotation, 
                ref _rotationVelocity, 
                referenceKeeper.PlayerData.RotationSmoothTime);

            // rotate to face input direction relative to camera position            
            referenceKeeper.PlayerData.CharacterController.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

    }
    #endregion
    #region Animation Event Section
    private void ComboCheckStart() {
        ListenToMeleeAttack = true;
        EnableCombo = false;
    }
    private void ComboCheckEnd()
    {
        ListenToMeleeAttack = false;        
    }
    private void AttackEnded()
    {
        CheckDeriveToOtherAttackStyle(deriveToThisStyle, true);

        // Check Reach to the last Combo
        if (!EnableCombo && referenceKeeper.PlayerData.CurrentCombo == GetCurrentLastCombo())
        {
            referenceKeeper.PlayerData.CurrentCombo = EnumHolder.ComboCounter.Combo1;
            InputCDProcess = StartCoroutine(InputCoolDown());
        }

        if (EnableCombo)        
        {
            SetAnimatorTrigger();

            // Chain to next combo
            ++referenceKeeper.PlayerData.CurrentCombo;

            EnableCombo = false;
        }
        else {
            ResetComboCheckStatus();            
        }
        if (referenceKeeper.PlayerData.EnableSlowMoWhenCheckTheseCombo.Length != 0) ResetTimeScale();
    }
    private void TurnOnDamageTrigger(int name) 
    {
        StartCoroutine(HitVFXTriggerLifeTime());
        StringToComboDamageTrigger(name).SetActive(true);
    }
    private void TurnOffDamageTrigger(int name)
    {
        StringToComboDamageTrigger(name).SetActive(false);
    }
    private GameObject StringToComboDamageTrigger(int name) {
        int resultIndex = name - 1;
                
        switch (referenceKeeper.PlayerData.CurrentAttackStyle)
        {
            case EnumHolder.AttackStyle.None:
                return referenceKeeper.PlayerData.Melee1ComboTrigger[resultIndex];
            case EnumHolder.AttackStyle.Melee1:
                return referenceKeeper.PlayerData.Melee1ComboTrigger[resultIndex];
            case EnumHolder.AttackStyle.Melee2:
                return referenceKeeper.PlayerData.Melee2ComboTrigger[resultIndex];
            case EnumHolder.AttackStyle.Melee3:
                return referenceKeeper.PlayerData.Melee3ComboTrigger[resultIndex];
            default:
                return referenceKeeper.PlayerData.Melee1ComboTrigger[resultIndex];                
        }
    }
    private void PlayComboVFX() {
        if (!AttackSuccess) return;

        ParticleSystem vfx = null;
        switch (referenceKeeper.PlayerData.CurrentAttackStyle)
        {
            case EnumHolder.AttackStyle.None:
                break;
            case EnumHolder.AttackStyle.Melee1:
                vfx = referenceKeeper.PlayerData.Melee1ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
            case EnumHolder.AttackStyle.Melee2:
                vfx = referenceKeeper.PlayerData.Melee2ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];                
                break;
            case EnumHolder.AttackStyle.Melee3:
                vfx = referenceKeeper.PlayerData.Melee3ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];                
                break;
            default:
                break;
        }              

        if (vfx) { CurrentAttackVFX = vfx; vfx.Stop(true); vfx.Play(true); }
    }
    #endregion
    #region Damage Section
    private void CheckAttackEnemySuccess(Collider target) { 
        IDamagable damagableTarget = target.GetComponentInParent<IDamagable>();
        damagableTarget.CheckAttackSuccess(out bool success);

        AttackSuccess = success;

        if (!AttackSuccess)
        {            
            HitTrigger.enabled = false;
            CurrentAttackVFX.Stop(true);
        }
    }
    public void CheckAttackSuccess(out bool success)
    {
        success = true;
    }
    private void DealDamage(Collider target) {
        IDamagable damagableTarget = target.GetComponentInParent<IDamagable>();

        if (damagableTarget == null) return;

        var comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee1ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];

        switch (referenceKeeper.PlayerData.CurrentAttackStyle)
        {
            case EnumHolder.AttackStyle.None:
                comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee1ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
            case EnumHolder.AttackStyle.Melee1:
                comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee1ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
            case EnumHolder.AttackStyle.Melee2:
                comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee2ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
            case EnumHolder.AttackStyle.Melee3:
                comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee3ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
            default:
                comboDetailList = referenceKeeper.PlayerData.AttackStyleSettings.m_Melee1ComboDetailsList[(int)referenceKeeper.PlayerData.CurrentCombo];
                break;
        }

        TimeManager.Instance.StartTimeEvent(comboDetailList.AttackFeedback.ToString());

        damagableTarget.OnReceiveDamage(
            comboDetailList.DamageAmount,
            comboDetailList.PushDistance,
            comboDetailList.PushDuration,
            comboDetailList.PushBackMovement, 
            referenceKeeper.PlayerData.CharacterController.transform);
        
        if (!AttackSuccess)
        {
            ReflectByShield(target);
        }
    }
    private void PlayHitVFX(Collider target)
    {
        if (AttackSuccess)
        {
            var cloneHitVFX = Instantiate(referenceKeeper.PlayerData.HitVFX, target.ClosestPoint(referenceKeeper.PlayerData.HitVFXTrigger.transform.position), Quaternion.identity, target.transform);
            Destroy(cloneHitVFX, 2f);
        }
    }
    #endregion
    #region Attack Assistance
    private void ChangeTarget(float horizontal, float vertical)
    {
        if (horizontal > -.5f && horizontal < .5 && vertical > -.5 && vertical < .5) return;
        if (referenceKeeper.PlayerData.SwitchTargetCD) return;
        
        SearchTarget();

        if (CurrentEnemies.Count <= 1) return;

        ChangeLockTarget();                
        SwitchToNextTarget(horizontal, vertical);
    }
    private Collider SearchTarget() {
        var result = Physics.OverlapSphere(
            referenceKeeper.PlayerData.CharacterController.transform.position, 
            referenceKeeper.PlayerData.SearchRadius,
            referenceKeeper.PlayerData.SearchLayer);
                       
        return OrderByFacingDegree(result);
    }    
    /// <summary>
    /// This func will arrange the targets and return the closet target
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    private Collider OrderByDistance(Collider[] targets) {
        if (targets.Length == 0) return null;
        
        Dictionary<Collider,float> Dist = new Dictionary<Collider, float>();
        var playerPos = referenceKeeper.PlayerData.CharacterController.transform.position;
        
        for (int i = 0; i < targets.Length; i++)
        {
            Dist.Add(targets[i],Vector3.Distance(playerPos, targets[i].transform.position));
        }
        
        var newResult = Dist.OrderBy(x => x.Value);

        CurrentEnemies.Clear();

        foreach (var kvp in newResult)
        {
            CurrentEnemies.Add(kvp.Key);
        }

        return CurrentEnemies[0];        
    }
    /// <summary>
    /// This func will arrange the targets and return the closet target
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    private Collider OrderByFacingDegree(Collider[] targets)
    {
        if (targets.Length == 0) return null;

        Dictionary<Collider, float> Angle = new Dictionary<Collider, float>();
        var playerPos = referenceKeeper.PlayerData.CharacterController.transform.position;
        var playerFacing = referenceKeeper.PlayerData._3rdPersonCamera.transform.forward;        
        Vector3 dir;

        for (int i = 0; i < targets.Length; i++)
        {
            dir = (targets[i].transform.position - playerPos).normalized;
            Angle.Add(targets[i], Vector3.Angle(playerFacing, dir));
        }

        var newResult = Angle.OrderBy(x => x.Value);

        CurrentEnemies.Clear();

        foreach (var kvp in newResult)
        {
            CurrentEnemies.Add(kvp.Key);
        }

        return CurrentEnemies[0];
    }
    private void SwitchToNextTarget(float horizontal, float vertical) {
        Dictionary<Collider, float> HorizontalTargetAngle = new Dictionary<Collider, float>();
        Dictionary<Collider, float> VerticalTargetAngle = new Dictionary<Collider, float>();
        SortedTarget.Clear();

        var playerPos = referenceKeeper.PlayerData.CharacterController.transform.position;
        var playerLeftRight =  (horizontal / Mathf.Abs(horizontal)) * referenceKeeper.PlayerData._3rdPersonCamera.transform.right;
        var playerUpDown =  (vertical / Mathf.Abs(vertical)) * referenceKeeper.PlayerData._3rdPersonCamera.transform.up;
        Vector3 dir;

        for (int i = 0; i < CurrentEnemies.Count; i++)
        {
            dir = (CurrentEnemies[i].transform.position - playerPos).normalized;
            HorizontalTargetAngle.Add(CurrentEnemies[i], Vector3.Angle(playerLeftRight, dir));
            VerticalTargetAngle.Add(CurrentEnemies[i], Vector3.Angle(playerUpDown, dir));
        }

        bool isHorizontal = true;

        if(Mathf.Abs(horizontal) > .5f)
            isHorizontal = true;
        //if(Mathf.Abs(vertical) > .5f)
        //    isHorizontal = false;        

        var newResult = (isHorizontal) ? HorizontalTargetAngle.OrderBy(x => x.Value): VerticalTargetAngle.OrderBy(x => x.Value);

        foreach (var kvp in newResult)
        {
            SortedTarget.Add(kvp.Key);
        }

        for (int i = 0; i < SortedTarget.Count; i++)
        {
            if (SortedTarget[i] == targetCollider) {
                if (i - 1 >= 0)
                { 
                    ChangeTargetCollider(SortedTarget[i - 1]); 
                    EventHandler.WhenLockOnTarget?.Invoke(SortedTarget[i - 1].transform); 
                }
            }
        }
    }
    private void ChangeTargetCollider(Collider newTarget) {
        targetCollider = newTarget;

        if(targetCollider != null)
            EventHandler.WhenAutoAimTarget?.Invoke(targetCollider.transform);
    }
    public void ActivateLockOnTarget()
    {
        UserControllerGetter.Instance.LockOnUpDelegate += LockOnTarget;
    }
    public void DeactivateLockOnTarget()
    {
        UserControllerGetter.Instance.LockOnUpDelegate -= LockOnTarget;
    }
    public void ActivateSwitchTarget()
    {
        UserControllerGetter.Instance.MouseInputDelegate += ChangeTarget;
    }
    public void DeactivateSwitchTarget()
    {
        UserControllerGetter.Instance.MouseInputDelegate -= ChangeTarget;
    }
    private void LockOnTarget() {
        if (!referenceKeeper.PlayerData.m_LockOn)
        {
            var target = SearchTarget();
            if (target == null) return;
            
            ChangeTargetCollider(target);

            EventHandler.WhenLockOnTarget?.Invoke(target.transform);
            referenceKeeper.PlayerData.m_LockOn = true;
                        
            DeactivateCamera();
            ActivateSwitchTarget();

            if (LockOnProcess != null)
                StopCoroutine(LockOnProcess);

            LockOnProcess = StartCoroutine(LockOnTarget(referenceKeeper.PlayerData.CharacterController.transform));
            referenceKeeper.AnimationPlayer.SetFloat(referenceKeeper.PlayerData.LockOnName, 1, referenceKeeper.PlayerData.LockOnAnimatorTrasitionDuration);
        }
        else 
        {
            referenceKeeper.PlayerData.m_LockOn = false;

            var rot = referenceKeeper.PlayerData.CinemachineCameraTarget.transform.eulerAngles;

            if (LockOnProcess != null)
                StopCoroutine(LockOnProcess);

            ChangeTargetCollider(null);

            _cinemachineTargetPitch = rot.x + referenceKeeper.PlayerData.CameraAngleOverride;
            _cinemachineTargetYaw = rot.y;

            //DOTween.To(
            //    () => referenceKeeper.PlayerData.CinemachineCameraTarget.transform.eulerAngles,
            //    x => referenceKeeper.PlayerData.CinemachineCameraTarget.transform.eulerAngles = x,
            //    rot, .3f);

            if (_cinemachineTargetPitch > referenceKeeper.PlayerData.TopClamp) _cinemachineTargetPitch -= 360;
            if (_cinemachineTargetPitch < referenceKeeper.PlayerData.BottomClamp) _cinemachineTargetPitch += 360;
           
            ActivateCamera();
            DeactivateSwitchTarget();

            EventHandler.WhenUnlockTarget?.Invoke();
            referenceKeeper.AnimationPlayer.SetFloat(referenceKeeper.PlayerData.LockOnName, 0, referenceKeeper.PlayerData.LockOnAnimatorTrasitionDuration);
        }        
    }    
    private void ChangeLockTarget() {
        if (LockOnProcess != null)
            StopCoroutine(LockOnProcess);

        LockOnProcess = StartCoroutine(LockOnTarget(referenceKeeper.PlayerData.CharacterController.transform));

        StartCoroutine(SwitchTargetCoolDown());
    }
    private IEnumerator LockOnTarget(Transform player) {
        Vector3 dir;
        Quaternion targetRot;
        float step = 0;
        var from = referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation;
        while (step < 1)
        {
            dir = targetCollider.transform.position - new Vector3(player.position.x, player.position.y - referenceKeeper.PlayerData.LockOnPitchOffset, player.position.z);
            dir = dir.normalized;

            targetRot = Quaternion.LookRotation(dir);
            step += Time.deltaTime * referenceKeeper.PlayerData.LockOnCamTransitionSpeed;
            referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation = Quaternion.Lerp(from, targetRot, step);

            yield return null;
        }

        var rot = referenceKeeper.PlayerData.CinemachineCameraTarget.transform.localEulerAngles;

        while (Vector3.Distance(targetCollider.transform.position, referenceKeeper.PlayerData.CharacterController.transform.position) < referenceKeeper.PlayerData.SearchRadius)
        {            
            dir = targetCollider.transform.position - new Vector3(player.position.x, player.position.y - referenceKeeper.PlayerData.LockOnPitchOffset, player.position.z);
            dir = dir.normalized;
            
            rot = referenceKeeper.PlayerData.CinemachineCameraTarget.transform.localEulerAngles;

            referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation = Quaternion.LookRotation(dir);
            
            _cinemachineTargetPitch = rot.x + referenceKeeper.PlayerData.CameraAngleOverride;
            _cinemachineTargetYaw = rot.y;

            yield return null;
        }

        LockOnTarget();
    }
    #endregion
    #region Hit    
    public void OnReceiveDamage(float damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker)
    {
        WhenGetDamage?.Invoke();
        EventHandler.WhenReceiveDamage?.Invoke();
        HealthModule.ChangeHealth(this, -damageAmount);
        OnHealthPercentageChanged(HealthPercentage);
        CharacterFacingEnemy(referenceKeeper.PlayerData.FacingEnemySpeed);
        SetAnimatorTrigger(referenceKeeper.PlayerData.HitKnockbackName);

        StartCoroutine(CombatCoroutines.CharacterControllerPositionLerping(attacker, referenceKeeper.PlayerData.CharacterController, pushBackDistance, duration, movement));
    }
    public void ReflectByShield(Collider target) {
        //referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.ReflectName);
        WhenGetDamage?.Invoke(); 
        EventHandler.WhenHitShield?.Invoke(target.ClosestPoint(referenceKeeper.PlayerData.HitVFXTrigger.transform.position), Quaternion.identity);
        TimeManager.Instance.StartTimeEvent("HitShield");
        SetAnimatorTrigger(referenceKeeper.PlayerData.ReflectName);
        //referenceKeeper.AnimationPlayer.HitShield();
    }
    #endregion
    #region Heal
    public void OnHeal(float damageAmount) {
        HealthModule.ChangeHealth(this, damageAmount);
        OnHealthPercentageChanged(HealthPercentage);
    }
    #endregion
    #region Coroutine Section   
    private IEnumerator HitVFXTriggerLifeTime() {
        referenceKeeper.PlayerData.HitVFXTrigger.SetActive(true);
        yield return new WaitForSeconds(.1f);
        referenceKeeper.PlayerData.HitVFXTrigger.SetActive(false);
    }
    private IEnumerator AttackCoolDown()
    {
        referenceKeeper.PlayerData.AttackCD = true;
        yield return new WaitForSeconds(referenceKeeper.PlayerData.AttackCDTime);
        referenceKeeper.PlayerData.AttackCD = false;
    }
    private IEnumerator InputCoolDown() {
        referenceKeeper.PlayerData.InputCD = true;
        yield return new WaitForSeconds(referenceKeeper.PlayerData.InputCDTime);
        referenceKeeper.PlayerData.InputCD = false;
    }
    private IEnumerator JumpCoolDown()
    {        
        referenceKeeper.PlayerData.JumpCD = true;
        yield return new WaitForSeconds(referenceKeeper.PlayerData.JumpCDTime);
        referenceKeeper.PlayerData.JumpCD = false;        
    }
    private IEnumerator DashCoolDown()
    {
        referenceKeeper.PlayerData.DashCD = true;
        yield return new WaitForSeconds(referenceKeeper.PlayerData.DashCDTime);
        referenceKeeper.PlayerData.DashCD = false;
    }
    private IEnumerator SwitchTargetCoolDown()
    {
        referenceKeeper.PlayerData.SwitchTargetCD = true;
        yield return new WaitForSeconds(referenceKeeper.PlayerData.SwitchTargetCDTime);
        referenceKeeper.PlayerData.SwitchTargetCD = false;
    }
    #endregion
    #region Time Control
    private void SlowDownComboTime() {
        for (int i = 0; i < referenceKeeper.PlayerData.EnableSlowMoWhenCheckTheseCombo.Length; i++)
        {
            if (referenceKeeper.PlayerData.CurrentCombo == referenceKeeper.PlayerData.EnableSlowMoWhenCheckTheseCombo[i])
            {
                TimeManager.Instance.StartTimeEvent("ComboCheck");
            }
        }
    }
    private void ResetTimeScale() {
        TimeManager.Instance.ResetTimeScale();
    }
    #endregion
    #region Animator Settings
    private void SetAnimatorFloat(float horizontal, float vertical)
    {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetFloat(referenceKeeper.PlayerData.RunHorizontalName, horizontal);
        referenceKeeper.AnimationPlayer.AnimatorRef.SetFloat(referenceKeeper.PlayerData.RunVerticalName, vertical);
    }
    private void SetAnimatorTrigger() {        
        switch (referenceKeeper.PlayerData.CurrentCombo)
        {
            case EnumHolder.ComboCounter.Combo1:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo1Name);
                break;
            case EnumHolder.ComboCounter.Combo2:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo2Name);
                break;
            case EnumHolder.ComboCounter.Combo3:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo3Name);
                break;
            case EnumHolder.ComboCounter.Combo4:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo4Name);
                break;
            case EnumHolder.ComboCounter.Combo5:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo5Name);
                break;
            case EnumHolder.ComboCounter.Combo6:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo6Name);
                break;
            case EnumHolder.ComboCounter.Combo7:
                referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.Combo7Name);
                break;
            default:
                break;
        }
    }
    private void SetAnimatorBool(string name, bool value)
    {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetBool(name, value);
    }
    private void SetAnimatorTrigger(string name) {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetTrigger(name);    
    }
    #endregion
    #region SMB Event
    public void OnStateExit(int damageTriggerIndex)
    {
        TurnOffDamageTrigger(damageTriggerIndex);

        AttackSuccess = true;
        HitTrigger.enabled = true;

        // Animation Ended
        DurationFromAttackEndedToStateExitWhenComboEnded = false;
        OnAttackStateExitDel?.Invoke();
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (referenceKeeper == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(referenceKeeper.PlayerData.CharacterController.transform.position + Vector3.up * referenceKeeper.PlayerData.CheckGroundedOffset, referenceKeeper.PlayerData.CheckGroundedSphereRadius);
        Gizmos.DrawSphere(referenceKeeper.PlayerData.CharacterController.transform.position - Vector3.up * referenceKeeper.PlayerData.CheckGroundedDist, referenceKeeper.PlayerData.CheckGroundedSphereRadius);
        
        //Debug.DrawLine(transform.position, transform.position + transform.up * referenceKeeper.PlayerData.JumpHight);

        //Gizmos.color = new Color(1,0,0,.3f);

        //Gizmos.DrawSphere(referenceKeeper.PlayerData.CharacterController.transform.position, referenceKeeper.PlayerData.SearchRadius);
    }
}
