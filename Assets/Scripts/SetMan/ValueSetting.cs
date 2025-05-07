using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Settings {

    public abstract class ValueSetting<T> : Setting {
        public T Value { get; set; }

        [SettingProperty] public T defaultValue;

        public UnityAction<T> onValueChanged;

        public override string Serialize() => JsonConvert.SerializeObject(Value);
        public override void Deserialize(string value) => JsonConvert.DeserializeObject<T>(value);
    }

}
