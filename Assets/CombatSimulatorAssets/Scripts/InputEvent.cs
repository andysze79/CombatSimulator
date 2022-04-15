using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class InputEvent : MonoBehaviour
{
    [SerializeField] private UserControllerGetter.ControllerDelegat m_TargetControl;
    public UltEvent OnTargetInputTriggered;

    private void OnEnable()
    {
        switch (m_TargetControl)
        {
            case UserControllerGetter.ControllerDelegat.MouseInputDelegate:
                UserControllerGetter.Instance.MouseInputDelegate += FloatTwoDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Joystick1InputDelegate:
                UserControllerGetter.Instance.Joystick1InputDelegate += FloatTwoDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.RunDownDelegate:
                UserControllerGetter.Instance.RunDownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.RunUpDelegate:
                UserControllerGetter.Instance.RunUpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.JumpDownDelegate:
                UserControllerGetter.Instance.JumpDownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.JumpUpDelegate:
                UserControllerGetter.Instance.JumpUpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.DashDownDelegate:
                UserControllerGetter.Instance.DashDownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.DashUpDelegate:
                UserControllerGetter.Instance.DashUpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.GuardDownDelegate:
                UserControllerGetter.Instance.GuardDownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.GuardUpDelegate:
                UserControllerGetter.Instance.GuardUpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.LockOnDownDelegate:
                UserControllerGetter.Instance.LockOnDownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.LockOnUpDelegate:
                UserControllerGetter.Instance.LockOnUpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight1DownDelegate:
                UserControllerGetter.Instance.Fight1DownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight1UpDelegate:
                UserControllerGetter.Instance.Fight1UpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight2DownDelegate:
                UserControllerGetter.Instance.Fight2DownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight2UpDelegate:
                UserControllerGetter.Instance.Fight2UpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3DownDelegate:
                UserControllerGetter.Instance.Fight3DownDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3UpDelegate:
                UserControllerGetter.Instance.Fight3UpDelegate += BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3Delegate:
                UserControllerGetter.Instance.Fight3Delegate += FloatDelegateTriggered;
                break;
            default:
                break;
        }
    }
    private void OnDisable()
    {
        switch (m_TargetControl)
        {
            case UserControllerGetter.ControllerDelegat.MouseInputDelegate:
                UserControllerGetter.Instance.MouseInputDelegate -= FloatTwoDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Joystick1InputDelegate:
                UserControllerGetter.Instance.Joystick1InputDelegate -= FloatTwoDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.RunDownDelegate:
                UserControllerGetter.Instance.RunDownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.RunUpDelegate:
                UserControllerGetter.Instance.RunUpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.JumpDownDelegate:
                UserControllerGetter.Instance.JumpDownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.JumpUpDelegate:
                UserControllerGetter.Instance.JumpUpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.DashDownDelegate:
                UserControllerGetter.Instance.DashDownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.DashUpDelegate:
                UserControllerGetter.Instance.DashUpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.GuardDownDelegate:
                UserControllerGetter.Instance.GuardDownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.GuardUpDelegate:
                UserControllerGetter.Instance.GuardUpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.LockOnDownDelegate:
                UserControllerGetter.Instance.LockOnDownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.LockOnUpDelegate:
                UserControllerGetter.Instance.LockOnUpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight1DownDelegate:
                UserControllerGetter.Instance.Fight1DownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight1UpDelegate:
                UserControllerGetter.Instance.Fight1UpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight2DownDelegate:
                UserControllerGetter.Instance.Fight2DownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight2UpDelegate:
                UserControllerGetter.Instance.Fight2UpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3DownDelegate:
                UserControllerGetter.Instance.Fight3DownDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3UpDelegate:
                UserControllerGetter.Instance.Fight3UpDelegate -= BoolDelegateTriggered;
                break;
            case UserControllerGetter.ControllerDelegat.Fight3Delegate:
                UserControllerGetter.Instance.Fight3Delegate -= FloatDelegateTriggered;
                break;
            default:
                break;
        }
    }
    private void BoolDelegateTriggered() 
    {
        OnTargetInputTriggered?.Invoke();
    }
    private void FloatDelegateTriggered(float value)
    {
        OnTargetInputTriggered?.Invoke();
    }
    private void FloatTwoDelegateTriggered(float horizontal, float vertical)
    {
        OnTargetInputTriggered?.Invoke();

        if(TryGetComponent<IUIFloatTwo>(out IUIFloatTwo uIFloatTwo)) uIFloatTwo.OnTriggered(horizontal, vertical);
    }
}
public interface IUIFloatTwo{
    public void OnTriggered(float horizontal, float vertical);
}
