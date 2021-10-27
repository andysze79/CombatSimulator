using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void OnReceiveDamage(int damageAmount, float pushBackDistance, float duration, AnimationCurve movement, Transform attacker);
}
