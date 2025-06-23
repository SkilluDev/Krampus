using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable {
    [SerializeField] private Transform m_switch;
    [SerializeField] private Light m_light;
    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private bool m_enabledByDefault = true;
    public Vector3 InteractionPoint => m_switch != null ? m_switch.position : transform.position;

    public int Priority => -15;

    private void Start() {
        if (!m_enabledByDefault) Toggle();
    }

    public void Toggle() {
        m_light.enabled = !m_light.enabled;
        if (m_light.enabled)
            m_renderer.material.EnableKeyword("_EMISSION");
        else
            m_renderer.material.DisableKeyword("_EMISSION");
    }

    public void Interact(IInteractor interactor) {
        Toggle();
    }
}
