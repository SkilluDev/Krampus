using System;
using UnityEngine;

public class QualityManager : MonoBehaviour {
	private int m_lastSettingValue = -999;

	private void OnEnable() {
		QualityChange();

		Game.GlobalEvents.onSetManChange.AddListener(OnSetManChange);
	}

	private void OnDisable() {
		Game.GlobalEvents.onSetManChange.RemoveListener(OnSetManChange);
	}

	private void OnSetManChange(string key) {
		if (key == "Quality") {
			QualityChange();
		}
	}

	private void QualityChange() {
		int value = (int)Game.SetMan.GetValue<long>("Quality");
		if (value == m_lastSettingValue) return;
		QualitySettings.SetQualityLevel(value, true);
		m_lastSettingValue = value;
	}
}

