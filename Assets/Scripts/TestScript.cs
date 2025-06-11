using System;
using UnityEngine;

public class TestScript : MonoBehaviour {
	[SerializeField] private KrampusStatDict m_customEnumDictionary;

	private void Update() {
		Debug.Log("Speed===" + m_customEnumDictionary[KrampusStats.Stat.Speed].Value);
	}

	private void OnValidate() {
		m_customEnumDictionary.Validate();
	}
}

[Serializable]
public class TestStat : ValueConnectedToEnum<KrampusStats.Stat> {
	[SerializeField] private float m_value;
	public float Value => m_value;

}

[Serializable]
public class KrampusStatDict : SerializedEnumDictionary<KrampusStats.Stat, TestStat> { }

