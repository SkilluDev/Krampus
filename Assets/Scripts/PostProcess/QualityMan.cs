using UnityEngine;

public class QualityManager : MonoBehaviour {
    private int m_lastSettingValue = -999;

    private void FixedUpdate() {
        int value = (int)Game.SetMan.GetValue<long>("Quality");
        if (value == m_lastSettingValue) return;

        QualitySettings.SetQualityLevel(value);
        m_lastSettingValue = value;


        Debug.Log("[Quality] Switch quality: " + value);
    }
}

