using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class UserControlExternalState : MonoBehaviour
{
    public bool m_UseFire1 = false;
    public ControllerInput Fire1Input = ControllerInput.Fire1;
    public UltEvent WhenFire1Down;
    public UltEvent WhenFire1Up;

    public bool m_UseFire2 = false;
    public ControllerInput Fire2Input = ControllerInput.Fire2;
    public UltEvent WhenFire2Down;
    public UltEvent WhenFire2Up;

    public bool m_UseFire3 = false;
    public ControllerInput Fire3Input = ControllerInput.Fire3;
    public UltEvent WhenFire3Down;
    public UltEvent WhenFire3Up;

    public void OnEnable()
    {
        if (m_UseFire1) UserControllerGetter.Instance.Fight1DownDelegate += Fire1Down; 
        if (m_UseFire2) UserControllerGetter.Instance.Fight2DownDelegate += Fire2Down; 
        if (m_UseFire3) UserControllerGetter.Instance.Fight3DownDelegate += Fire3Down; 
        if (m_UseFire1) UserControllerGetter.Instance.Fight1UpDelegate += Fire1Up; 
        if (m_UseFire2) UserControllerGetter.Instance.Fight2UpDelegate += Fire2Up; 
        if (m_UseFire3) UserControllerGetter.Instance.Fight3UpDelegate += Fire3Up; 
    }

    public void Fire1Down()
    {
        WhenFire1Down.Invoke();
    }
    public void Fire2Down()
    {
        WhenFire2Down.Invoke();
    }
    public void Fire3Down()
    {
        WhenFire3Down.Invoke();
    }
    public void Fire1Up()
    {
        WhenFire1Up.Invoke();
    }
    public void Fire2Up()
    {
        WhenFire2Up.Invoke();
    }
    public void Fire3Up()
    {
        WhenFire3Up.Invoke();
    }
}
