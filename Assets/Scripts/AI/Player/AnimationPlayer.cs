using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationPlayer : MonoBehaviour
{
    public delegate void AnimationPlayerEvent();
    public delegate void AnimationEventCombo(int name);
    public AnimationPlayerEvent WhenStartCheckAnimationEvent;
    public AnimationPlayerEvent WhenPlayComboVFX;
    public AnimationPlayerEvent WhenCheckComboStart;
    public AnimationPlayerEvent WhenCheckComboEnd;
    public AnimationPlayerEvent WhenAttackEnded;
    public AnimationEventCombo WhenTurnOnDamageTrigger;
    public AnimationEventCombo WhenTurnOffDamageTrigger;
    // Enemy
    public AnimationPlayerEvent WhenCheckAttackPosition;
    private int CurrentLayerIndex { get; set; }
    public Animator AnimatorRef { get; set; }
    Coroutine settingProcess { get; set; }
    //public ReferenceKeeper referenceKeeper { get; set; }
    protected virtual void Awake()
    {
        AnimatorRef = GetComponent<Animator>();
        //referenceKeeper = GetComponentInParent<ReferenceKeeper>();
    }
    public void PlayAnimation(string name) {        
        AnimatorRef.SetTrigger(name);
    }
    public void PlayAnimation(string name, bool value) {
        AnimatorRef.SetBool(name, value);
    }
    public void PlayComboVFX() {
        WhenPlayComboVFX?.Invoke();
    }
    public void ResetTrigger(string name) {
        AnimatorRef.ResetTrigger(name);
    }
    public void StartCheckAnimationEvent() {
        WhenStartCheckAnimationEvent?.Invoke();
    }
    #region Animation Events
    public void TurnOnDamageTrigger(int comboName) 
    {
        WhenTurnOnDamageTrigger?.Invoke(comboName);
    }
    public void TurnOffDamageTrigger(int comboName)
    {
        WhenTurnOffDamageTrigger?.Invoke(comboName);
    }
    public void CheckAttackPosition() {
        WhenCheckAttackPosition?.Invoke();
    }
    #endregion
    public void CheckComboStart() {
        WhenCheckComboStart?.Invoke();
    }
    public void CheckComboEnd()
    {
        WhenCheckComboEnd?.Invoke();
    }
    public void AttackEnded()
    {
        WhenAttackEnded?.Invoke();
    }
    public void SetFloat(string name, float to, float duration) {  
        float value = AnimatorRef.GetFloat(name);
        DG.Tweening.DOTween.To(() => value, x => value = x, to, duration).OnUpdate(() => {
            AnimatorRef.SetFloat(name, value);
        });
    }
    public void SetFloat(string name, float to)
    {
        AnimatorRef.SetFloat(name, to);
    }
    public void HitShield() { 
        StartCoroutine(SettingFloatImpulse("Direction", 1, -0.7f, .8f));        
    }
    public void StepForward(float direction) {
        StartCoroutine(SettingFloatImpulse("RunSpeed",0,direction,0));
        //StartCoroutine(ChangePosition(direction));
    }
    public void ChangeLayer(int layerIndex, float layerWeight, float duration) {
        if (CurrentLayerIndex == layerIndex && AnimatorRef.GetLayerWeight(layerIndex) == layerWeight) return;
        CurrentLayerIndex = layerIndex;
        DOTween.To(ChangingLayer, AnimatorRef.GetLayerWeight(layerIndex), layerWeight, duration);
    }
    private void ChangingLayer(float layerWeight)
    {
        AnimatorRef.SetLayerWeight(CurrentLayerIndex, layerWeight);
    }
    private IEnumerator SettingFloatImpulse(string name, float from, float to, float duration)
    {
        AnimatorRef.SetFloat(name, to);

        yield return new WaitForSeconds(duration);
        yield return new WaitForEndOfFrame();

        AnimatorRef.SetFloat(name, from);
    }
    private IEnumerator ChangePosition(float direction) {
        AnimatorRef.SetFloat("Runspeed", direction);
        
        yield return new WaitForEndOfFrame();

        AnimatorRef.SetFloat("Runspeed", 0);
    }
}
