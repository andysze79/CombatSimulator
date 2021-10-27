using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
public enum ControllerInput{
    Horizontal, Vertical, Fire1, Fire2, Fire3
}

public class UserControllerGetter : MonoBehaviour
{
    public enum Controller {
        Keyboard
    }
    public enum InputMode { 
        Controller, UI
    }

    public Controller m_Controller = Controller.Keyboard;
    public InputMode m_InputMode = InputMode.Controller;
    [Header("Control by Input")]
    [SerializeField] private string m_MouseX = "MouseX";
    [SerializeField] private string m_MouseY = "MouseY";
    [SerializeField] private string m_Horizontal = "Horizontal";
    [SerializeField] private string m_Vertical = "Vertical";
    [SerializeField] private string m_JumpName = "Jump";
    [SerializeField] private string m_DashName = "Dash";
    [SerializeField] private string m_LockOnName = "LockOn";
    [SerializeField] private string m_Fight1Name = "Fire1";
    [SerializeField] private string m_Fight2Name = "Fire2";
    [SerializeField] private string m_Fight3Name = "Fire3";
    [Header("Control by Timeline")]
    [Range(-1, 1)] public float m_HorizontalController;
    [Range(-1, 1)] public float m_VerticalController;
    public bool m_Fire1Controller;

    [Header("Events")]
    public UltEvent WhenJoyStickNoInput;
    public UltEvent WhenFire1Up;
    public float MouseX { get; set; }
    public float MouseY { get; set; }
    public float HorizontalValue { get; set; }
    public float VerticalValue { get; set; }
    public float JumpValue { get; set; }
    public float DashValue { get; set; }
    public float LockOnValue { get; set; }
    public bool Fight1Value { get; set; }
    public bool Fight2Value { get; set; }
    public bool Fight3Value { get; set; }

    public delegate void Joystick1Input(float HorizontalValue, float VerticalValue);
    public Joystick1Input MouseInputDelegate; 
    public Joystick1Input Joystick1InputDelegate; 
    public delegate void Fight1();
    public Fight1 JumpDownDelegate;
    public Fight1 JumpUpDelegate;
    public Fight1 DashDownDelegate;
    public Fight1 DashUpDelegate; 
    public Fight1 LockOnDownDelegate;
    public Fight1 LockOnUpDelegate;
    public Fight1 Fight1DownDelegate;
    public Fight1 Fight1UpDelegate;
    public delegate void Fight2();
    public Fight2 Fight2DownDelegate;
    public Fight2 Fight2UpDelegate;
    public delegate void Fight3();
    public Fight3 Fight3DownDelegate;
    public Fight3 Fight3UpDelegate;

    public float HorizontalController { get; set; }
    public float VerticalController { get; set; }
    public bool Fire1Controller { get; set; }


    public static UserControllerGetter m_Instance;
    public static UserControllerGetter Instance {
        get {
            if (m_Instance != null)
            {
                return m_Instance;
            }
            else {
                m_Instance = GameObject.FindObjectOfType<UserControllerGetter>();
                return m_Instance;
            }
        }
    }
       

    public void Update()
    {
        #region Control by player
        if (m_InputMode == InputMode.Controller)
        {
            if (m_Controller == Controller.Keyboard)
            {
                MouseX = Input.GetAxis(m_MouseX);

                MouseY = Input.GetAxis(m_MouseY);

                MouseInputDelegate?.Invoke(MouseX, MouseY);

                HorizontalValue = Input.GetAxisRaw(m_Horizontal);

                VerticalValue = Input.GetAxisRaw(m_Vertical);

                Joystick1InputDelegate?.Invoke(HorizontalValue, VerticalValue);
                //if (HorizontalValue != 0 || VerticalValue != 0)
                //    Joystick1InputDelegate?.Invoke(HorizontalValue, VerticalValue);
                //else
                //{
                //    WhenJoyStickNoInput?.Invoke();
                //}                
                if (Input.GetButtonDown(m_JumpName)) {                     
                    JumpDownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_JumpName)) {
                    JumpUpDelegate?.Invoke();
                }
                if (Input.GetButtonDown(m_DashName)) {
                    DashDownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_DashName))
                {
                    DashUpDelegate?.Invoke();
                }
                if (Input.GetButtonDown(m_LockOnName))
                {
                    LockOnDownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_LockOnName))
                {
                    LockOnUpDelegate?.Invoke();
                }
                if (Input.GetButtonDown(m_Fight1Name))
                {
                    Fight1DownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_Fight1Name))
                {
                    Fight1UpDelegate?.Invoke();
                    WhenFire1Up.Invoke();
                }
                if (Input.GetButtonDown(m_Fight2Name))
                {
                    Fight2DownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_Fight2Name))
                {
                    Fight2UpDelegate?.Invoke();
                }
                if (Input.GetButtonDown(m_Fight3Name))
                {
                    Fight3DownDelegate?.Invoke();
                }
                if (Input.GetButtonUp(m_Fight3Name))
                {
                    Fight3UpDelegate?.Invoke();
                }
            }
        }
        #endregion
    }

    public void ChangeInputMode(InputMode mode) {
        m_InputMode = mode;
    }

}
