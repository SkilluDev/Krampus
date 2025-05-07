using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsRenderer : MonoBehaviour {
    [SerializeField] private RectTransform m_rt;
    private void Start() {
        float currentY = 0;
        foreach (var setting in Game.SetMan.Settings) {
            var settingRoot = new GameObject(setting.Name);
            settingRoot.transform.SetParent(m_rt);
            var settingRt = settingRoot.GetComponent<RectTransform>();
            settingRt.anchoredPosition = new Vector2(0, -currentY);
            settingRt.anchorMax = new Vector2(0f, 0.5f);
            settingRt.anchorMin = new Vector2(1f, 0.5f);
        }
    }
}
