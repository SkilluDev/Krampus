using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[Serializable]
public class SerializedEnumDictionary<TKey, TValue>: ISerializationCallbackReceiver, IReadOnlyDictionary<TKey, TValue> where TKey : Enum where TValue : ValueConnectedToEnum<TKey>, new() {

	[SerializeField] private List<TValue> m_values = new List<TValue>();

	private Dictionary<TKey, TValue> m_dictionary;

	public IEnumerable<TKey> Keys => m_values.Select(v=>v.Key);

	public IEnumerable<TValue> Values => m_values;

	public int Count => m_values.Count;

	public TValue this[TKey key] => m_dictionary[key];

	public SerializedEnumDictionary() {
		PopulateList();
		PopulateDictionary();
	}

	public void Validate() {
		ValidateList();
		ValidateDictionary();
	}

	private void ValidateList() {
		var enums = Enum.GetValues(typeof(TKey));
		var enumList = new List<TKey>();
		foreach (TKey e in enums) {
			enumList.Add(e);
			if (Keys.Contains(e)) continue;
			TValue newValue = new TValue();
			newValue.Key = e;
			m_values.Add(newValue);
		}
		var toRemove = new List<TValue>();
		var alreadyChecked = new List<TKey>();
		foreach (var v in m_values) {
			if (!enumList.Contains(v.Key)) {
				toRemove.Add(v);
				continue;
			}
			if (!alreadyChecked.Contains(v.Key)) {
				alreadyChecked.Add(v.Key);
				continue;
			} else {
				toRemove.Add(v);
				continue;
			}
		}
		foreach (var v in toRemove) {
			m_values.Remove(v);
		}
		toRemove.Clear();
		alreadyChecked.Clear();
	}

	private void ValidateDictionary() {
		foreach (var v in m_values) {
			if (m_dictionary.ContainsKey(v.Key)) continue;
			m_dictionary.Add(v.Key, v);
		}
		var toRemove = new List<TKey>();
		foreach (var k in m_dictionary.Keys) {
			if (Keys.Contains(k)) continue;
			toRemove.Add(k);
		}

		foreach (var key in toRemove) {
			m_dictionary.Remove(key);
		}
		toRemove.Clear();
	}

	private void PopulateList() {
		var enums = Enum.GetValues(typeof(TKey));
		foreach (TKey e in enums) {
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

	public bool ContainsKey(TKey key) => m_dictionary.ContainsKey(key);
	public bool TryGetValue(TKey key, out TValue value) => m_dictionary.TryGetValue(key,out value);
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_dictionary.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public void OnBeforeSerialize() => Validate();
	public void OnAfterDeserialize() => Validate();
}

#if UNITY_EDITOR
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
#endif
