using UnityEngine;

public class SettingsRenderer : MonoBehaviour {
    [SerializeField] private RectTransform m_rt;
    private void Start() {
        float currentY = 0;
        foreach (var setting in Game.SetMan.Settings) {
            var settingRoot = setting.CreateInstance(Game.SetMan, transform);
            var settingRt = settingRoot.GetComponent<RectTransform>();
            settingRt.anchoredPosition = new Vector2(0, -currentY);
            currentY += settingRt.rect.height;
        }
    }
}
