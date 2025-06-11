using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[Serializable]
public class SerializedEnumDictionary<TKey, TValue>: IReadOnlyDictionary<TKey, TValue> where TKey : Enum where TValue : ValueConnectedToEnum<TKey>, new() {

	[SerializeField] private List<TValue> m_values = new List<TValue>();

	private List<TKey> m_keys = new List<TKey>();

	private Dictionary<TKey, TValue> m_dictionary;

	public IEnumerable<TKey> Keys => m_keys;

	public IEnumerable<TValue> Values => m_values;

	public int Count => m_keys.Count;

	public TValue this[TKey key] => m_dictionary[key];

	public SerializedEnumDictionary() {
		PopulateList();
		PopulateDictionary();
	}

	public void PopulateList() {
		var enums = Enum.GetValues(typeof(TKey));
		foreach (TKey e in enums) {
			m_keys.Add(e);
			TValue newValue = new TValue();
			newValue.Key = e;
			m_values.Add(newValue);
		}
	}
	private void PopulateDictionary() {
		m_dictionary = new Dictionary<TKey, TValue>();
		if (m_values == null) return;
		foreach (var value in m_values) {
			if (!m_dictionary.ContainsKey(value.Key)) {
				m_dictionary.Add(value.Key, value);
			} else {
				Debug.LogError($"Duplicate Key '{value.Key}' found in list. Using the first entry.");
			}
		}
	}

	public bool ContainsKey(TKey key) => throw new NotImplementedException();
	public bool TryGetValue(TKey key, out TValue value) => throw new NotImplementedException();
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new NotImplementedException();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


[CustomPropertyDrawer(typeof(SerializedEnumDictionary<,>), true)]
public class SerializedEnumDictDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty values = property.FindPropertyRelative("m_values");

        if (values != null) {
            EditorGUI.PropertyField(position, values, label, true);
        } else {
            EditorGUI.LabelField(position, "No values found.");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty values = property.FindPropertyRelative("m_values");
        return values != null ? EditorGUI.GetPropertyHeight(values, true) : base.GetPropertyHeight(property, label);
    }
}

