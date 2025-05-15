using System.Reflection;
using Sound;
using UnityEngine;

public class FeetSounds : MonoBehaviour {
	[SerializeField] private Sex m_stepSoundBite;

	[SerializeField] private MonoBehaviour m_toInspect;
	[SerializeField] private bool m_filterOutState;
	private PropertyInfo m_stateField;

	private void Awake() {
		m_stateField = m_toInspect.GetType().GetProperty("CurrentState");
	}

	private void Step(string state) {
		string val = m_stateField.GetValue(m_toInspect).ToString();
		Debug.Log($"Play step {state} == {val}");
		if (val.Equals(state, System.StringComparison.InvariantCultureIgnoreCase) || !m_filterOutState)
			m_stepSoundBite.Play(transform.position);
	}
}
