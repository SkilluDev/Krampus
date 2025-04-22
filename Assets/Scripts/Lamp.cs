using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lamp : MonoBehaviour, IInteractable {
    [SerializeField] private Transform m_switch;
    [SerializeField] private Light m_light;
    [SerializeField] private MeshRenderer m_renderer;


    public Vector3 InteractionPoint => m_switch != null ? m_switch.position : transform.position;

    public void Interact(IInteractor interactor) {
        m_light.enabled = !m_light.enabled;
        if (m_light.enabled)
            m_renderer.material.EnableKeyword("_EMISSION");
        else
            m_renderer.material.DisableKeyword("_EMISSION");
    }
}
