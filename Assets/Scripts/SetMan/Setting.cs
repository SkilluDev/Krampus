using UnityEngine;

namespace Settings {
    public abstract class Setting : MonoBehaviour, ISerializationCallbackReceiver {
        [SerializeField] private string m_serialized;
        [SettingProperty] public new string name;

        public abstract string Serialize();
        public abstract void Deserialize(string value);
        public void OnBeforeSerialize() => m_serialized = Serialize();
        public void OnAfterDeserialize() => Deserialize(m_serialized);
    }
}