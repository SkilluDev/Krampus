using System;
using Settings;
using TMPro;
using UnityEngine;

namespace Settings {

    public abstract class TextInputSetting<T> : ValueSetting<T> {
        [SerializeField] private TMP_InputField m_valueField;

        public override void Cook() {
            base.Cook();
            m_valueField.onValidateInput += ValidateInputValue;
            m_valueField.onValueChanged.AddListener(InputValueChanged);
            m_valueField.SetTextWithoutNotify(manager.GetValue(name).ToString());
        }

        protected virtual char ValidateInputValue(string text, int charIndex, char addedChar) {
            return addedChar;
        }

        protected abstract void InputValueChanged(string newValue);
    }
}