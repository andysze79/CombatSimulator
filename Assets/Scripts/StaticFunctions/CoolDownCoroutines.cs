using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class CoolDownCoroutines
{

    public static IEnumerator CoolDownAction(UnityAction from, UnityAction to, float duration) {
        from.Invoke();
        yield return new WaitForSeconds(duration);
        to.Invoke();        
    }
}
