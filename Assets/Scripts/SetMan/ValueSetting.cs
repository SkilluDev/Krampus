using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Settings {

    public abstract class ValueSetting<T> : Setting {
        [SettingProperty] public T defaultValue;
        [SerializeField] private TextMeshProUGUI m_label;
        public UnityAction<T> onValueChanged;

        public override void Cook() {
            m_label.SetText(name);
        }
    }

}
