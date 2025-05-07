using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Settings {
    public class BoolSetting : ValueSetting<bool> {
        [SerializeField] private Toggle m_toggle;

        public override void Cook() {
            base.Cook();
            m_toggle.isOn = manager.GetValue<bool>(name);
            m_toggle.onValueChanged.AddListener(InputValueChanged);
        }

        protected virtual void InputValueChanged(bool value) {
            manager.SetValue<bool>(name, value);
        }
    }
}
