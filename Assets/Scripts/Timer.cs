using NaughtyAttributes;
using UnityEngine;

public class Timer : MonoBehaviour {
	[BoxGroup("Timer")][SerializeField] private int m_timeBonus;
	[BoxGroup("Timer")][SerializeField] private int m_timePenalty;
	[ShowNativeProperty] public float GameTime { get; private set; } = 10f;

	public void Ready() {
		GameTime = (int)Game.SetMan.GetValue<long>("Timer");
	}

	private void Update() {
		GameTime -= Time.deltaTime;
	}

	public void ChildBonus(ChildType childType) {
		if (childType == Game.MainGameInfo.GoodChildType) {
			Penalty();
		} else {
			Bonus();
		}
	}

	public void Bonus() {
		GameTime += m_timeBonus;
	}

	public void Penalty() {
		GameTime -= m_timePenalty;
	}
}
