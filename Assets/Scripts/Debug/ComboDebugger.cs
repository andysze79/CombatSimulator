using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboDebugger : MonoBehaviour
{
    public GameObject m_AbleToComboSign;
    public GameObject m_DisableToComboSign;
    public GameObject m_DeriveToMelee1Text;
    public GameObject m_DeriveToMelee2Text;    
    public TMPro.TextMeshProUGUI m_Text;
    public PlayerLogic playerLogic { get; set; }
    private void Start()
    {
        StartCoroutine(Delayer());

        m_DeriveToMelee1Text.SetActive(false);
        m_DeriveToMelee2Text.SetActive(false);
    }
    private IEnumerator Delayer()
    {
        yield return new WaitForSeconds(.5f);

        playerLogic = GameObject.FindObjectOfType<PlayerLogic>();

        if (playerLogic)
        {
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboStart += AbleToCombo;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboStart += EnableDeriveHint;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboEnd += DisableCombo;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboEnd += DisableDeriveHint;
        }
    }
    private void OnDisable()
    {        
        if (playerLogic)
        {
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboStart -= AbleToCombo;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboStart -= EnableDeriveHint;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboEnd -= DisableCombo;
            playerLogic.referenceKeeper.AnimationPlayer.WhenCheckComboEnd -= DisableDeriveHint;
        }
    }
    
    private void AbleToCombo() {
        m_AbleToComboSign.SetActive(true);
        m_DisableToComboSign.SetActive(false);
    }
    private void DisableCombo()
    {
        m_AbleToComboSign.SetActive(false);
        m_DisableToComboSign.SetActive(true);

        m_Text.text = playerLogic.referenceKeeper.PlayerData.CurrentCombo.ToString();
    }
    private void EnableDeriveHint() {
        if (playerLogic.referenceKeeper.PlayerData.CurrentAttackStyle == EnumHolder.AttackStyle.Melee1 &&
            playerLogic.referenceKeeper.PlayerData.CurrentCombo == playerLogic.referenceKeeper.PlayerData.Melee1ToMelee2[0])
        {
            m_DeriveToMelee2Text.SetActive(true);
        }
        if (playerLogic.referenceKeeper.PlayerData.CurrentAttackStyle == EnumHolder.AttackStyle.Melee2 &&
            playerLogic.referenceKeeper.PlayerData.CurrentCombo == playerLogic.referenceKeeper.PlayerData.Melee2ToMelee1[0])
        {
            m_DeriveToMelee1Text.SetActive(true);
        }
    }
    private void DisableDeriveHint() {
        if(m_DeriveToMelee2Text.activeSelf) m_DeriveToMelee2Text.SetActive(false);
        if(m_DeriveToMelee1Text.activeSelf) m_DeriveToMelee1Text.SetActive(false);
    }
}
