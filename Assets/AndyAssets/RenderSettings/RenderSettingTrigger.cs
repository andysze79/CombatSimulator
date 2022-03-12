using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerBase))]
public class RenderSettingTrigger : MonoBehaviour
{
    public void ChangeRenderSetting(RenderSettingsEnum value) {
        RendersettingsSwitcher.Instance.ChangeRenderSettings(value);
    }
    public void ChangeRenderSetting(string value) {
        RendersettingsSwitcher.Instance.ChangeRenderSettings(value);
    }
}
