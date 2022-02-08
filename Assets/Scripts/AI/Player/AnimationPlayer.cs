using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Animator AnimatorRef { get; set; }
    //public ReferenceKeeper ReferenceKeeper { get; set; }
    protected virtual void Awake()
    {
        AnimatorRef = GetComponent<Animator>();
        //ReferenceKeeper = GetComponentInParent<ReferenceKeeper>();
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
        //WhenStartCheckAnimationEvent?.Invoke();
    }
    public void TurnOnDamageTrigger(int comboName) 
    {
        WhenTurnOnDamageTrigger?.Invoke(comboName);
    }
    public void TurnOffDamageTrigger(int comboName)
    {
        WhenTurnOffDamageTrigger?.Invoke(comboName);
    }
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
    public void StepForward(float direction) {
        StartCoroutine(ChangePosition(direction));
    }
    private IEnumerator ChangePosition(float direction) {
        AnimatorRef.SetFloat("Runspeed", direction);
        
        yield return new WaitForEndOfFrame();

        AnimatorRef.SetFloat("Runspeed", 0);
    }
}
