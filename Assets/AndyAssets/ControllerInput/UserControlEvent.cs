using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using Sirenix.OdinInspector;

public class UserControlEvent : MonoBehaviour
{
    public ControllerInput m_ListenInput = ControllerInput.Fire1;    
    public UltEvent WhenAnalogInput;
    public UltEvent WhenDown;
    public UltEvent WhenUp;

    public void OnEnable()
    {
        switch (m_ListenInput)
        {
            case ControllerInput.Horizontal:
                break;
            case ControllerInput.Vertical:
                break;
            case ControllerInput.Fire1:
                UserControllerGetter.Instance.Fight1DownDelegate += DoDownEvent;
                UserControllerGetter.Instance.Fight1UpDelegate += DoUpEvent;
                break;
            case ControllerInput.Fire2:
                UserControllerGetter.Instance.Fight2DownDelegate += DoDownEvent;
                UserControllerGetter.Instance.Fight2UpDelegate += DoUpEvent;
                break;
            case ControllerInput.Fire3:
                UserControllerGetter.Instance.Fight3DownDelegate += DoDownEvent;
                UserControllerGetter.Instance.Fight3UpDelegate += DoUpEvent;
                break;
            default:
                break;
        }
    }
    public void OnDisable()
    {
        switch (m_ListenInput)
        {
            case ControllerInput.Horizontal:
                break;
            case ControllerInput.Vertical:
                break;
            case ControllerInput.Fire1:
                UserControllerGetter.Instance.Fight1DownDelegate -= DoDownEvent;
                UserControllerGetter.Instance.Fight1UpDelegate -= DoUpEvent;
                break;
            case ControllerInput.Fire2:
                UserControllerGetter.Instance.Fight2DownDelegate -= DoDownEvent;
                UserControllerGetter.Instance.Fight2UpDelegate -= DoUpEvent;
                break;
            case ControllerInput.Fire3:
                UserControllerGetter.Instance.Fight3DownDelegate -= DoDownEvent;
                UserControllerGetter.Instance.Fight3UpDelegate -= DoUpEvent;
                break;
            default:
                break;
        }
    }
    public void DoDownEvent()
    {
        WhenDown.Invoke();
    }
    public void DoUpEvent()
    {
        WhenUp.Invoke();
    }
}
