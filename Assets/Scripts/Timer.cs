using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class Timer : MonoBehaviour
{
	[BoxGroup("Timer")][SerializeField] private int m_timeBonus;
	[BoxGroup("Timer")][SerializeField] private int m_timePenalty;

	private float m_time = 10f;

	[ShowNativeProperty]public float GameTime {
		get => m_time;
		private set=>m_time = value;
	}

	public void Ready() {
		GameTime = (int)Game.SetMan.GetValue<long>("Timer");
	}
	private void Update() {
		Debug.Log(m_time);
		m_time -= Time.deltaTime;
	}

	public void childBonus(ChildType childType) {
		if (childType == Game.MainGameInfo.GoodChildType) {
			Penalty();
		} else {

			Bonus();
		}
	}

	public void Bonus() {
		m_time += m_timeBonus;
	}

	public void Penalty() {
		m_time -= m_timePenalty;
	}
}
