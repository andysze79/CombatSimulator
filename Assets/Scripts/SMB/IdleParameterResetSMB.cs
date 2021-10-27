using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleParameterResetSMB : StateMachineBehaviour
{
    public PlayerDataHolder m_PlayerDataHolder;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerDataHolder.CurrentAttackStyle = EnumHolder.AttackStyle.None;
        m_PlayerDataHolder.CurrentCombo = EnumHolder.ComboCounter.Combo1;
    }
}
