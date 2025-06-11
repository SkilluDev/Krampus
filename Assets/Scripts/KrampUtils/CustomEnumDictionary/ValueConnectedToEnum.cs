using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ValueConnectedToEnum<TKey> where TKey : Enum {
	[SerializeField, ReadOnly] private TKey m_key;
	public TKey Key { get => m_key; set => m_key = value; }

	public ValueConnectedToEnum() {
	}
}

[CustomPropertyDrawer(typeof(ValueConnectedToEnum<>), true)]
public class ValueConnectedToEnumDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty keyProperty = property.FindPropertyRelative("m_key");
        string enumName = keyProperty.enumDisplayNames[keyProperty.enumValueIndex];

        EditorGUI.PropertyField(position, property, new GUIContent(enumName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
