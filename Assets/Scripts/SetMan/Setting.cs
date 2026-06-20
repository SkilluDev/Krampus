using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Settings {
    public abstract class Setting : MonoBehaviour {
        [SettingProperty] public new string name;
        public SetMan manager;
        public virtual void Cook() { }
    }
}