using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AttackStyleSettings", menuName = "CombatSimulator/ScriptableObjects/AttackStyleSettings", order = 2)]
public class AttackStyleSettings : ScriptableObject
{
    public string m_Weapon;
    [System.Serializable]
    public struct ComboDetails
    {        
        public int DamageAmount;
        public float PushDistance;
        public float PushDuration;
        public AnimationCurve PushBackMovement;
    }
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<ComboDetails> m_Melee1ComboDetailsList = new List<ComboDetails>();
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<ComboDetails> m_Melee2ComboDetailsList = new List<ComboDetails>();
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<ComboDetails> m_Melee3ComboDetailsList = new List<ComboDetails>();
}
