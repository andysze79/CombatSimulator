using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void CheckAttackSuccess (out bool success);
    void OnReceiveDamage(float damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker);
}
