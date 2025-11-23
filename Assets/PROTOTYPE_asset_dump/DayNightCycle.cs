using System;
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
	public CyclePhase CurrentPhase => m_currentPhase;

	[SerializeField] private float m_dayTime = 30;

	public float DayTime => m_dayTime;
	[SerializeField] private float m_nightTime = 30;
	public float NightTime => m_nightTime;
	private float m_timer;
	private void Ready()
    {
		m_currentPhase = CyclePhase.Day;
		Game.GlobalEvents.onLevelStateChanged.AddListener(WaitForGameStart);
    }

	private void WaitForGameStart(MainGameInfo.State prev, MainGameInfo.State next) {
		if (next == MainGameInfo.State.Game) {
			Debug.Log("[DayNightCycle] Game started, beginning cycle");
			ChangePhase(CyclePhase.Night);
        }
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
		Debug.Log($"[DayNightCycle] Changing phase from {m_currentPhase} to {newPhase}");
		if(newPhase == m_currentPhase) {
			return;
		}
		CyclePhase oldPhase = m_currentPhase;
		m_currentPhase = newPhase;
		m_timer = 0;
		onCyclePhaseChanged.Invoke(oldPhase, newPhase);
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
