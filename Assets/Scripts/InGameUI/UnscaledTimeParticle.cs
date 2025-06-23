using UnityEngine;

public class UnscaledTimeParticle : MonoBehaviour
{
	[SerializeField] private ParticleSystem m_particleSystem;
	// Update is called once per frame
	private void Update() {
		if (Time.timeScale < 0.01f) {
			m_particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
		} else {
			m_particleSystem.Simulate(Time.deltaTime, true, false);
		}
	}
}
