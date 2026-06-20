using SaintsField.Playa;
using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable {
    [SerializeField] private Transform m_switch;
    [SerializeField] private Light[] m_lights;
    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private bool m_enabledByDefault = true;
    [ShowInInspector] public bool IsOn { get; private set; }
    public Vector3 InteractionPoint => m_switch != null ? m_switch.position : transform.position;

    public int Priority => -15;

    private void Start() {
        Toggle(m_enabledByDefault);
    }

    public void Toggle(bool newState) {
        IsOn = newState;
        foreach (var light in m_lights) {
            light.enabled = IsOn;
        }

        if (IsOn)
            m_renderer.material.EnableKeyword("_EMISSION");
        else
            m_renderer.material.DisableKeyword("_EMISSION");
    }

    public void Interact(IInteractor interactor) {
        Toggle(!IsOn);
    }
}
