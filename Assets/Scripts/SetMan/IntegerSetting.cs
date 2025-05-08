using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Settings {
    public class IntegerSetting : TextInputSetting<long> {
        protected override void InputValueChanged(string newValue) {
            manager.SetValue<long>(name, long.Parse(newValue));
        }
    }

}