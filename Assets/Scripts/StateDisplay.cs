using System.Reflection;
using TMPro;
using UnityEngine;

public class StateDisplay : MonoBehaviour {
    [SerializeField] private TextMeshPro m_statusText;

    [SerializeField] private MonoBehaviour m_toInspect;

    private PropertyInfo m_stateField;

    private void Awake() {
        m_stateField = m_toInspect.GetType().GetProperty("CurrentState");
    }

    private void Update() {
        m_statusText.transform.rotation = Quaternion.LookRotation(transform.position - Game.MainGameInfo.Krampus.Kamera.Rendering.transform.position);
        m_statusText.SetText(m_stateField.GetValue(m_toInspect).ToString());
    }
}
