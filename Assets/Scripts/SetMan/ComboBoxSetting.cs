using TMPro;
using UnityEngine;

namespace Settings {
    public class ComboBoxSetting : ValueSetting<int> {
        [SettingProperty] public string[] options;
        [SerializeField] private TMP_Dropdown m_dropdown;

        public override void Cook() {
            base.Cook();
            foreach (string w in options) m_dropdown.options.Add(new TMP_Dropdown.OptionData(w));
            m_dropdown.onValueChanged.AddListener(InputValueChanged);
        }

        protected virtual void InputValueChanged(int newValue) {
            manager.SetValue<int>(name, newValue);
        }
    }
}
