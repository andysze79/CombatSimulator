using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using Sirenix.OdinInspector;

public class PlayerDataHolder : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera m_3rdPersonCamera;
    [SerializeField] private float m_RotationSmoothTime = .12f;
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    [Header("Move State")]
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_RotateSpeed;
    [SerializeField] private float m_Gravity = -9.8f;
    [SerializeField] private string m_RunHorizontalName;
    [SerializeField] private string m_RunVerticalName;
    [SerializeField] private float m_CheckGroundedSphereRadius;
    [SerializeField] private LayerMask m_CheckGroundedLayer;
    [SerializeField] private float m_JumpHight;
    [SerializeField] private float m_JumpGravityMultiplier = 2f;
    [SerializeField] private float m_JumpCDTime;
    [Header("Dash State")]
    [SerializeField] private float m_DashCDTime;
    [Header("Attack State")]
    public bool m_LockOn;
    [SerializeField] private float m_LockOnPitchOffset;
    [SerializeField] private bool m_InputCD;
    [SerializeField] private float m_InputCDTime;    
    [SerializeField] private float m_AttackCDTime = 1f;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_DashName;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_JumpName;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_isGroundedName;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Melee1Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Melee2Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Melee3Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_ChargeName;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo1Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo2Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo3Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo4Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo5Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo6Name;
    [FoldoutGroup("Animator Trigger Name")]
    [SerializeField] private string m_Combo7Name;

    [FoldoutGroup("Combo Setting")]
    [SerializeField] private EnumHolder.ComboCounter m_Melee1LastCombo;
    [FoldoutGroup("Combo Setting")]
    [SerializeField] private EnumHolder.ComboCounter m_Melee2LastCombo;
    [FoldoutGroup("Combo Setting")]
    [SerializeField] private EnumHolder.ComboCounter m_Melee3LastCombo;
    [FoldoutGroup("AttackStyle Derive Setting")]
    [SerializeField] private EnumHolder.ComboCounter[] m_Melee1ToMelee2;
    [FoldoutGroup("AttackStyle Derive Setting")]
    [SerializeField] private EnumHolder.ComboCounter[] m_Melee2ToMelee1;
    [FoldoutGroup("AttackStyle Derive Setting")]
    [SerializeField] private float m_StartChargeTime;
    [FoldoutGroup("AttackStyle Derive Setting")]
    [SerializeField] private float m_ChargeTime;

    // TODO: Move to Scriptable Object
    [FoldoutGroup("Damage Settings")]
    [SerializeField]private AttackStyleSettings m_AttackStyleSettings;
    [FoldoutGroup("Damage Settings")]
    public bool m_EnableGizmos;
    [FoldoutGroup("Damage Settings")][ShowIf("m_EnableGizmos")]
    public int m_ShowThisComboPushBackDistance;

    [FoldoutGroup("Attack Assistance Settings")]
    public float m_FindEnemyRadius;    

    [FoldoutGroup("Trigger Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private GameObject[] m_Melee1ComboTrigger;
    [FoldoutGroup("Trigger Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private GameObject[] m_Melee2ComboTrigger;
    [FoldoutGroup("Trigger Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private GameObject[] m_Melee3ComboTrigger;
    [FoldoutGroup("Trigger Settings")]
    [SerializeField] private GameObject m_HitVFXTrigger;

    [FoldoutGroup("VFX Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private ParticleSystem[] m_Melee1ComboVFX;
    [FoldoutGroup("VFX Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private ParticleSystem[] m_Melee2ComboVFX;
    [FoldoutGroup("VFX Settings")][ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private ParticleSystem[] m_Melee3ComboVFX;
    [FoldoutGroup("VFX Settings")]
    [SerializeField] private GameObject m_HitVFX;
    [FoldoutGroup("VFX Settings")]
    public UltEvent m_WhenStartCharge;
    [FoldoutGroup("VFX Settings")]
    public UltEvent m_WhenFinsihedCharge;
    [FoldoutGroup("VFX Settings")]
    public UltEvent m_WhenReleaseCharge;

    [SerializeField] private EnumHolder.ComboCounter[] m_EnableSlowMoWhenCheckTheseCombo;
    [FoldoutGroup("Debug Monitor Section")]
    [SerializeField] private EnumHolder.AttackStyle m_CurrentAttackStyle = EnumHolder.AttackStyle.None;
    [FoldoutGroup("Debug Monitor Section")]
    [SerializeField] private EnumHolder.ComboCounter m_CurrentCombo;

    public Camera _3rdPersonCamera { get { return m_3rdPersonCamera; } }
    public float RotationSmoothTime { get { return m_RotationSmoothTime; } }
    public CharacterController CharacterController { get { return m_CharacterController; } }
    public float MoveSpeed { get { return m_MoveSpeed; } }
    public float RotateSpeed { get { return m_RotateSpeed; } }
    public float Gravity { get { return m_Gravity; } }
    public string RunHorizontalName { get { return m_RunHorizontalName; } }
    public string RunVerticalName { get { return m_RunVerticalName; } }
    public float CheckGroundedSphereRadius { get { return m_CheckGroundedSphereRadius; } }
    public LayerMask CheckGroundedLayer { get { return m_CheckGroundedLayer; } }    
    public float JumpHight { get { return m_JumpHight; } }    
    public float JumpGravityMultiplier { get {return m_JumpGravityMultiplier; } }
    public float JumpCDTime { get {return m_JumpCDTime; } }
    public float DashCDTime { get {return m_DashCDTime; } }
    public bool JumpCD { get; set; }
    public bool DashCD { get; set; }
    public float LockOnPitchOffset { get { return m_LockOnPitchOffset; } }
    public bool InputCD { get { return m_InputCD; } set { m_InputCD = value; } }
    public float InputCDTime { get { return m_InputCDTime; } }
    public EnumHolder.AttackStyle CurrentAttackStyle { get { return m_CurrentAttackStyle; }set { m_CurrentAttackStyle = value; } }
    public string DashName { get { return m_DashName; } }
    public string JumpName { get { return m_JumpName; } }
    public string isGroundedName { get { return m_isGroundedName; } }
    public string Melee1Name { get { return m_Melee1Name; } }
    public string Melee2Name { get { return m_Melee2Name; } }
    public string Melee3Name { get { return m_Melee3Name; } }
    public string ChargeName { get { return m_ChargeName; } }
    public string Combo1Name { get { return m_Combo1Name; } }
    public string Combo2Name { get { return m_Combo2Name; } }
    public string Combo3Name { get { return m_Combo3Name; } }
    public string Combo4Name { get { return m_Combo4Name; } }
    public string Combo5Name { get { return m_Combo5Name; } }
    public string Combo6Name { get { return m_Combo6Name; } }
    public string Combo7Name { get { return m_Combo7Name; } }    
    public float AttackCDTime { get { return m_AttackCDTime; } }    
    public GameObject[] Melee1ComboTrigger { get { return m_Melee1ComboTrigger; } }
    public GameObject[] Melee2ComboTrigger { get { return m_Melee2ComboTrigger; } }
    public GameObject[] Melee3ComboTrigger { get { return m_Melee3ComboTrigger; } }
    public GameObject HitVFXTrigger { get { return m_HitVFXTrigger; } }
    public AttackStyleSettings AttackStyleSettings { get { return m_AttackStyleSettings; } }
    public ParticleSystem[] Melee1ComboVFX { get { return m_Melee1ComboVFX; } }
    public ParticleSystem[] Melee2ComboVFX { get { return m_Melee2ComboVFX; } }
    public ParticleSystem[] Melee3ComboVFX { get { return m_Melee3ComboVFX; } }
    public GameObject HitVFX { get { return m_HitVFX; } }
    public EnumHolder.ComboCounter[] EnableSlowMoWhenCheckTheseCombo { get { return m_EnableSlowMoWhenCheckTheseCombo; } }
    public bool AttackCD { get; set; }
    public EnumHolder.ComboCounter CurrentCombo { get { return m_CurrentCombo; } set { m_CurrentCombo = value; } }
    public EnumHolder.ComboCounter Melee1LastCombo { get { return m_Melee1LastCombo; } }
    public EnumHolder.ComboCounter Melee2LastCombo { get { return m_Melee2LastCombo; } }
    public EnumHolder.ComboCounter Melee3LastCombo { get { return m_Melee3LastCombo; } }
    public EnumHolder.ComboCounter[] Melee1ToMelee2 { get { return m_Melee1ToMelee2; } }
    public EnumHolder.ComboCounter[] Melee2ToMelee1 { get { return m_Melee2ToMelee1; } }
    public float StartChargeTime { get { return m_StartChargeTime; } }
    public float ChargeTime { get { return m_ChargeTime; } }
    
    public void OnDrawGizmos()
    {
        if (m_EnableGizmos)
        {
            var comboDetailList = m_AttackStyleSettings.m_Melee1ComboDetailsList[m_ShowThisComboPushBackDistance];
            switch (CurrentAttackStyle)
            {
                case EnumHolder.AttackStyle.None:
                    comboDetailList = m_AttackStyleSettings.m_Melee1ComboDetailsList[m_ShowThisComboPushBackDistance];
                    break;
                case EnumHolder.AttackStyle.Melee1:
                    comboDetailList = m_AttackStyleSettings.m_Melee1ComboDetailsList[m_ShowThisComboPushBackDistance];
                    break;
                case EnumHolder.AttackStyle.Melee2:
                    comboDetailList = m_AttackStyleSettings.m_Melee2ComboDetailsList[m_ShowThisComboPushBackDistance];
                    break;
                case EnumHolder.AttackStyle.Melee3:
                    comboDetailList = m_AttackStyleSettings.m_Melee3ComboDetailsList[m_ShowThisComboPushBackDistance];
                    break;
                default:
                    comboDetailList = m_AttackStyleSettings.m_Melee1ComboDetailsList[m_ShowThisComboPushBackDistance];
                    break;
            }

            Debug.DrawLine(
                transform.position + new Vector3(0, 1, 0), 
                transform.position + new Vector3(0,1,0) + transform.forward * comboDetailList.PushDistance, 
                Color.blue);            
        }
    }
}
