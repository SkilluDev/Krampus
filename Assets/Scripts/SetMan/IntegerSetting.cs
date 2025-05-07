using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Settings {
    public class IntegerSetting : TextInputSetting<int> {
        protected override void InputValueChanged(string newValue) {
            manager.SetValue<int>(name, int.Parse(newValue));
        }
    }

}