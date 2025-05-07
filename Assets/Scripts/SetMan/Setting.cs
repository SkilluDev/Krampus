using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Settings {
    [Serializable]
    public abstract class Setting {
        public string name;
        public abstract RectTransform Create(RectTransform parent);
#if UNITY_EDITOR
        public abstract void OnInspectorGUI();
#endif
    }

    [Serializable]
    public abstract class ValueSetting<T> : Setting {
        public T Value {
            get => m_value;
            set {
                onValueChanged?.Invoke(m_value, value);
                m_value = value;
            }
        }
        [NonSerialized] protected T m_value;
        [NonSerialized] public UnityAction<T, T> onValueChanged;

    }

    [Serializable]
    public class IntegerSetting : ValueSetting<int> {
        public override RectTransform Create(RectTransform parent) => throw new NotImplementedException();

#if UNITY_EDITOR
        public override void OnInspectorGUI() {
            name = EditorGUILayout.TextField("Name", name);
        }
#endif
    }
}