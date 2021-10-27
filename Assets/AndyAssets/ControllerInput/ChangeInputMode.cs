using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeInputMode : MonoBehaviour
{
    public void ChangeInputModeToUI() {
        UserControllerGetter.Instance.ChangeInputMode(UserControllerGetter.InputMode.UI);
    }
    public void ChangeInputModeToController()
    {
        UserControllerGetter.Instance.ChangeInputMode(UserControllerGetter.InputMode.Controller);
    }
}
