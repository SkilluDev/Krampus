using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable, IDayNightCycleReactor {
    [SerializeField] private Transform m_switch;
    [SerializeField] private Light m_light;
    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private bool m_enabledByDefault = true;
    public Vector3 InteractionPoint => m_switch != null ? m_switch.position : transform.position;

    public int Priority => -15;

    private void Start() {
        if (!m_enabledByDefault) Toggle();
    }

	public void TestLamp() {
		if (!m_light) {
            Debug.LogError("[Lamp] No light component assigned!", this);
			Debug.DrawLine(transform.position, transform.position + Vector3.up * 20, Color.red, 20f);
        }
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

	public void TurnOn() {
		TestLamp();
		if (!m_light.enabled) {
			Toggle();
		}
	}

	public void TurnOff() {
		TestLamp();
		if (m_light.enabled) {
			Toggle();
		}
	}

	public void React(DayNightCycle.CyclePhase oldPhase, DayNightCycle.CyclePhase newPhase){
		if(newPhase == DayNightCycle.CyclePhase.Night) {
			TurnOff();
		} else if(newPhase == DayNightCycle.CyclePhase.Day) {
			TurnOn();
		}
	}
}
