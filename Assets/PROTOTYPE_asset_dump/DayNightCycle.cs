using UnityEngine;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
	public enum CyclePhase {
		Day,
		Night
	}

	public UnityEvent<CyclePhase, CyclePhase> onCyclePhaseChanged;

	private CyclePhase m_currentPhase;

	[SerializeField] private float m_dayTime = 30;
	[SerializeField] private float m_nightTime = 30;

	private float m_timer;
	private void Start()
    {
		m_currentPhase = CyclePhase.Night;
		Debug.Log("Starting at Night");
    }

	private void Update()
    {
		m_timer += Time.deltaTime;
		switch (m_currentPhase) {
			case CyclePhase.Day:
				if (m_timer >= m_dayTime) {
					m_timer = 0;
					NextPhase();
				}
				break;
			case CyclePhase.Night:
				if (m_timer >= m_nightTime) {
					m_timer = 0;
					NextPhase();
				}
				break;
			default:
				break;
        }
    }

	private void ChangePhase(CyclePhase newPhase) {
		if(newPhase == m_currentPhase) {
			return;
		}
		CyclePhase oldPhase = m_currentPhase;
		m_currentPhase = newPhase;
		onCyclePhaseChanged.Invoke(oldPhase, newPhase);
		Debug.Log("Changed phase from " + oldPhase.ToString() + " to " + newPhase.ToString());
    }

	private void NextPhase() {
		switch (m_currentPhase) {
			case CyclePhase.Day:
				ChangePhase(CyclePhase.Night);
				break;
			case CyclePhase.Night:
				ChangePhase(CyclePhase.Day);
				break;
			default:
				break;
        }
	}

}
