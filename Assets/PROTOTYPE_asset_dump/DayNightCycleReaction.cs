using System;
using UnityEngine;

public interface IDayNightCycleReactor {
	public void React(DayNightCycle.CyclePhase oldPhase, DayNightCycle.CyclePhase newPhase);
}

public class DayNightCycleReaction : MonoBehaviour {
	private DayNightCycle m_cycle;

	private IDayNightCycleReactor[] m_reactors;


	private void Awake() {
		m_reactors = GetComponents<IDayNightCycleReactor>();
		m_cycle = Game.MainGameInfo.DayNightCycle;
	}

	private void Start() {
		m_cycle.onCyclePhaseChanged.AddListener(OnCycleChange);
	}

	private void OnCycleChange(DayNightCycle.CyclePhase oldPhase, DayNightCycle.CyclePhase newPhase) {
		foreach (IDayNightCycleReactor reactor in m_reactors) {
			reactor.React(oldPhase, newPhase);
        }
	}
}
