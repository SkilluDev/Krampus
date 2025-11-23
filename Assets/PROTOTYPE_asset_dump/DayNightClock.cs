using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class DayNightClock : MonoBehaviour, IDayNightCycleReactor
{
	private float m_timer;

	private float m_timePerSegment = float.MaxValue;
	private int m_segments = 6;

	private bool m_isReady = false;

	[SerializeField] private AnimationCurve m_easeCurve;
	public void React(DayNightCycle.CyclePhase oldPhase, DayNightCycle.CyclePhase newPhase) {
		if(!m_isReady) {
			m_isReady = true;
		}
		Debug.Log($"[DayNightClock] Reacting to phase change from {oldPhase} to {newPhase}");
        if (newPhase == DayNightCycle.CyclePhase.Day) {
			m_timePerSegment = Game.MainGameInfo.DayNightCycle.DayTime/m_segments;
        } else if (newPhase == DayNightCycle.CyclePhase.Night) {
			m_timePerSegment = Game.MainGameInfo.DayNightCycle.NightTime/m_segments;
        }

		Debug.Log($"[DayNightClock] Timer set to {m_timer}, time per segment set to {m_timePerSegment}");
	}

	// Update is called once per frame
	private void Update()
    {
		if(!m_isReady) {
			return;
		}
		m_timer -= Time.deltaTime;
		if(m_timer < 0) {
			Debug.Log($"[DayNightClock] Rotating clock by {180/m_segments} degrees, over {m_timePerSegment} seconds");
			LMotion.Create(transform.localRotation, transform.localRotation * Quaternion.Euler(new Vector3(0, 0, 180/m_segments)), m_timePerSegment).WithEase(m_easeCurve).BindToLocalRotation(this.transform);
			m_timer = m_timePerSegment;
        }
	}
}
