using UnityEngine;

public class QualityManager : MonoBehaviour {
    private int m_lastSettingValue = -999;

    private void FixedUpdate() {
        int value = (int)Game.SetMan.GetValueIntegral("Quality");
        if (value == m_lastSettingValue) return;

        QualitySettings.SetQualityLevel(m_lastSettingValue);
        m_lastSettingValue = value;

        Debug.Log("Switch quality: " + value);
    }
}

