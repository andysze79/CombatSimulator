﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerLogic : MonoBehaviour, IMatchTarget, IMatchSurface
{
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
    Coroutine jumpProcess;
    EnumHolder.AttackStyle previousAttackStyle;
    EnumHolder.AttackStyle deriveToThisStyle;
    private float _rotationVelocity;
    private float _targetRotation; 
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    Coroutine LockOnProcess { get; set; }
    private void Update()
    {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetBool(referenceKeeper.PlayerData.isGroundedName, CheckGrounded());
    }

    #region Assign Delegate
    private void Start()
    {
        referenceKeeper = GetComponent<ReferenceKeeper>();

        referenceKeeper.AnimationPlayer.WhenCheckComboStart += ComboCheckStart;
        referenceKeeper.AnimationPlayer.WhenCheckComboStart += SlowDownComboTime;
        referenceKeeper.AnimationPlayer.WhenCheckComboEnd += ComboCheckEnd;
        referenceKeeper.AnimationPlayer.WhenAttackEnded += AttackEnded;
        referenceKeeper.AnimationPlayer.WhenTurnOnDamageTrigger += TurnOnDamageTrigger;
        referenceKeeper.AnimationPlayer.WhenTurnOffDamageTrigger += TurnOffDamageTrigger;

        referenceKeeper.AnimationPlayer.WhenPlayComboVFX += PlayComboVFX;

        #region Assign Combo Trigger Event
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee1ComboTrigger, true);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee2ComboTrigger, true);
        AssignTriggerDelegate(referenceKeeper.PlayerData.Melee3ComboTrigger, true);
        #endregion

        if (referenceKeeper.PlayerData.HitVFXTrigger.TryGetComponent(out DamageTrigger HitTrigger)) 
        {            
            HitTrigger.TriggerEnter += PlayHitVFX;
        }

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

        if (referenceKeeper.PlayerData.HitVFXTrigger.TryGetComponent(out DamageTrigger HitTrigger))
        {
            HitTrigger.TriggerEnter -= PlayHitVFX;
        }

        DeactivateCamera();
        DeactivateMove();
        DeactivateLockOnTarget();
    }
    private void AssignTriggerDelegate(GameObject[] meleeComboTrigger, bool AssignOrRelease) {
        DamageTrigger trigger;
        List<GameObject> AssignedTrigger = new List<GameObject>();

        for (int i = 0; i < meleeComboTrigger.Length; i++)
        {
            if (CheckAlreadyAssignedDelegate(AssignedTrigger, meleeComboTrigger[i])) continue;
            AssignedTrigger.Add(meleeComboTrigger[i]);

            trigger = meleeComboTrigger[i].GetComponent<DamageTrigger>();

            if(AssignOrRelease)
                trigger.TriggerEnter += DealDamage;
            else
                trigger.TriggerEnter -= DealDamage;
        }
    }
    private bool CheckAlreadyAssignedDelegate(List<GameObject> assignedList, GameObject target) {
        var result = false;

        for (int i = 0; i < assignedList.Count; i++)
        {
            if (assignedList[i] == target)
                result = true;
        }

        return result;
    }
    #endregion

    #region Input Receiver
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
    public void TriggerAttack(EnumHolder.AttackStyle attackStyle, string AnimatorTriggerName, EnumHolder.ComboCounter lastCombo) {

        // Check Derive to other Attack Style
        if (referenceKeeper.PlayerData.CurrentAttackStyle != EnumHolder.AttackStyle.None &&
            attackStyle != referenceKeeper.PlayerData.CurrentAttackStyle && 
            !CheckDeriveToOtherAttackStyle(attackStyle, false)) return;

        if (referenceKeeper.PlayerData.AttackCD) return;
        if (referenceKeeper.PlayerData.InputCD) return;
        if (referenceKeeper.PlayerData.CurrentCombo == lastCombo) return;
        
        InputCDProcess = StartCoroutine(InputCoolDown());

        //print("Trigger attack");

        // When start a new combo
        if (referenceKeeper.PlayerData.CurrentCombo == EnumHolder.ComboCounter.Combo1)
            ChangeAttackStyle(attackStyle);

        // When outside the combo check range
        if (!ListenToMeleeAttack)
        {
            referenceKeeper.AnimationPlayer.PlayAnimation(AnimatorTriggerName);
        }
        // When within the combo check range
        else
        {
            if(referenceKeeper.PlayerData.EnableSlowMoWhenCheckTheseCombo.Length != 0) ResetTimeScale();
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
    #endregion
    #region Camera
    private void ActivateCamera() {
        UserControllerGetter.Instance.MouseInputDelegate += CameraRotation;
        //UserControllerGetter.Instance.MouseInputDelegate += CheckCameraMovement;
    }
    private void DeactivateCamera()
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
                    print("from 1 to 2");
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
            print("Derive " + referenceKeeper.PlayerData.CurrentCombo);
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
            SetAnimationTrigger();

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
        ParticleSystem vfx;
        switch (referenceKeeper.PlayerData.CurrentAttackStyle)
        {
            case EnumHolder.AttackStyle.None:
                break;
            case EnumHolder.AttackStyle.Melee1:
                vfx = referenceKeeper.PlayerData.Melee1ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];
                if (vfx) vfx.Stop(true); vfx.Play(true); 
                break;
            case EnumHolder.AttackStyle.Melee2:
                vfx = referenceKeeper.PlayerData.Melee2ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];
                if (vfx) vfx.Stop(true); vfx.Play(true);
                break;
            case EnumHolder.AttackStyle.Melee3:
                vfx = referenceKeeper.PlayerData.Melee3ComboVFX[(int)referenceKeeper.PlayerData.CurrentCombo];
                if (vfx) vfx.Stop(true); vfx.Play(true);
                break;
            default:
                break;
        }              
    }    
    #endregion

    #region Damage Section
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


        damagableTarget.OnReceiveDamage(
            comboDetailList.DamageAmount,
            comboDetailList.PushDistance,
            comboDetailList.PushDuration,
            comboDetailList.PushBackMovement, 
            referenceKeeper.PlayerData.CharacterController.transform);
    }
    private void PlayHitVFX(Collider target)
    {
        var cloneHitVFX = Instantiate(referenceKeeper.PlayerData.HitVFX, target.ClosestPoint(referenceKeeper.PlayerData.HitVFXTrigger.transform.position), Quaternion.identity, target.transform);
        Destroy(cloneHitVFX, 2f);
    }
    #endregion

    #region Attack Assistance
    private Collider SearchTarget() {
        return null;        
    }
    private void ActivateLockOnTarget()
    {
        UserControllerGetter.Instance.LockOnUpDelegate += LockOnTarget;
    }
    private void DeactivateLockOnTarget()
    {
        UserControllerGetter.Instance.LockOnUpDelegate -= LockOnTarget;
    }
    private void LockOnTarget() {        
        if (!referenceKeeper.PlayerData.m_LockOn)
        {          
            referenceKeeper.PlayerData.m_LockOn = true;

            DeactivateCamera();

            if (LockOnProcess != null)
                StopCoroutine(LockOnProcess);

            LockOnProcess = StartCoroutine(LockOnTarget(referenceKeeper.PlayerData.CharacterController.transform));
        }
        else 
        {
            referenceKeeper.PlayerData.m_LockOn = false;

            var rot = referenceKeeper.PlayerData.CinemachineCameraTarget.transform.localEulerAngles;
            print(rot);

            if (LockOnProcess != null)
                StopCoroutine(LockOnProcess);

            _cinemachineTargetPitch = rot.x - referenceKeeper.PlayerData.CameraAngleOverride;
            _cinemachineTargetYaw = rot.y;

            referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation = 
                Quaternion.Euler(
                    _cinemachineTargetPitch + referenceKeeper.PlayerData.CameraAngleOverride, 
                    _cinemachineTargetYaw, 
                    0.0f);
            
            ActivateCamera();
        }        
    }
    private IEnumerator LockOnTarget(Transform target) {
        Vector3 dir;

        while (true)
        {            
            dir = targetCollider.transform.position - new Vector3(target.position.x, target.position.y - referenceKeeper.PlayerData.LockOnPitchOffset, target.position.z);
            dir = dir.normalized;
            

            referenceKeeper.PlayerData.CinemachineCameraTarget.transform.rotation = Quaternion.LookRotation(dir);

            yield return null;
        }
    }
    #endregion
    private void SetAnimationTrigger() {        
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
    private void ResetComboCheckStatus() {
        referenceKeeper.PlayerData.CurrentCombo = 0;
        ChangeAttackStyle(EnumHolder.AttackStyle.None);
        AttackCDProcess = StartCoroutine(AttackCoolDown());
    }
    
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
    #region Moving
    private void ActivateMove() {        
        UserControllerGetter.Instance.Joystick1InputDelegate += CheckMove;        
        UserControllerGetter.Instance.Joystick1InputDelegate += CheckRotate;        
        UserControllerGetter.Instance.Joystick1InputDelegate += SetAnimatorFloat;

        UserControllerGetter.Instance.JumpUpDelegate += CheckJump;
        UserControllerGetter.Instance.DashUpDelegate += CheckDash;
    }
    private void DeactivateMove() {
        UserControllerGetter.Instance.Joystick1InputDelegate -= CheckMove;
        UserControllerGetter.Instance.Joystick1InputDelegate -= CheckRotate;        
        UserControllerGetter.Instance.Joystick1InputDelegate -= SetAnimatorFloat;        

        UserControllerGetter.Instance.JumpUpDelegate -= CheckJump;
        UserControllerGetter.Instance.DashUpDelegate -= CheckDash;
    }    
    private void CheckMove(float horizontal, float vertical) {

        var moveValue = new Vector3(horizontal, 0, vertical);

        var ChangeToCameraAlginment = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
                
        moveValue = ChangeToCameraAlginment * moveValue;
        moveValue = moveValue * Time.deltaTime * referenceKeeper.PlayerData.MoveSpeed;

        // Gravity
        moveValue.y = referenceKeeper.PlayerData.Gravity * Time.deltaTime;

        referenceKeeper.PlayerData.CharacterController.Move(moveValue);
    }
    private void CheckRotate(float horizontal, float vertical) {

        if (horizontal == 0 && vertical == 0) return;

        var moveDir = new Vector3(horizontal, 0, vertical);

        var ChangeToCameraAlginment = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        var targetDir = ChangeToCameraAlginment * moveDir;

        var newRot = Quaternion.LookRotation(targetDir);

        var ratio = Quaternion.Angle(referenceKeeper.PlayerData.CharacterController.transform.rotation, newRot);
        ratio = (ratio / 360) + 1;
        ratio *= 2;

        referenceKeeper.PlayerData.CharacterController.transform.rotation = Quaternion.RotateTowards(
            referenceKeeper.PlayerData.CharacterController.transform.rotation,
            newRot,
            Time.deltaTime * referenceKeeper.PlayerData.RotateSpeed * ratio);
    }
    private void SetAnimatorFloat(float horizontal, float vertical) {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetFloat(referenceKeeper.PlayerData.RunHorizontalName, horizontal);
        referenceKeeper.AnimationPlayer.AnimatorRef.SetFloat(referenceKeeper.PlayerData.RunVerticalName, vertical);
    }
    #region Jump
    private bool CheckGrounded() {
        return Physics.CheckSphere(
            referenceKeeper.PlayerData.CharacterController.transform.position, 
            referenceKeeper.PlayerData.CheckGroundedSphereRadius,
            referenceKeeper.PlayerData.CheckGroundedLayer);        
    }
    
    private void CheckJump()
    {
        if (!CheckGrounded() || referenceKeeper.PlayerData.JumpCD) return;

        if (jumpProcess != null)
            StopCoroutine(jumpProcess);

        jumpProcess = StartCoroutine(JumpProcess());

        SetAnimatorTrigger(referenceKeeper.PlayerData.JumpName);

        referenceKeeper.PlayerData.JumpCD = true;
    }
    
    private IEnumerator JumpProcess()
    {        
        var startTime = Time.time;
        var velocity = Mathf.Sqrt(referenceKeeper.PlayerData.JumpHight * -2f * referenceKeeper.PlayerData.Gravity * referenceKeeper.PlayerData.JumpGravityMultiplier);

        while (true)
        {
            velocity += referenceKeeper.PlayerData.Gravity * referenceKeeper.PlayerData.JumpGravityMultiplier * Time.deltaTime;
            referenceKeeper.PlayerData.CharacterController.Move(new Vector3(0,velocity,0) * Time.deltaTime);

            if (startTime > .5f && CheckGrounded()) break;

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

        StartCoroutine(DashCoolDown());
    }
    #endregion
    #endregion


    private void SetAnimatorBool(string name, bool value)
    {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetBool(name, value);
    }
    private void SetAnimatorTrigger(string name) {
        referenceKeeper.AnimationPlayer.AnimatorRef.SetTrigger(name);    
    }
    private void OnDrawGizmos()
    {
        if (referenceKeeper == null) return;

        Gizmos.color = Color.red;

        Debug.DrawLine(transform.position, transform.position + transform.up * referenceKeeper.PlayerData.JumpHight);

        Gizmos.DrawWireSphere(referenceKeeper.PlayerData.CharacterController.transform.position, referenceKeeper.PlayerData.CheckGroundedSphereRadius);
    }
}
