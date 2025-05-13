using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour {
	[BoxGroup("Timer")][SerializeField] private int m_timeBonus;
	[BoxGroup("Timer")][SerializeField] private int m_timePenalty;

	[ShowNativeProperty] public float GameTime { get; private set; } = 10f;

	private void OnChildEaten(ChildType childType) {
		if (childType == Game.MainGameInfo.GoodChildType) {
			Penalty();
		} else {
			Bonus();
		}
	}

	public void Ready() {
		GameTime = (int)Game.SetMan.GetValue<long>("Timer");
		Game.MainGameInfo.GlobalEvents.onChildEaten += OnChildEaten;
		//hey
	}

	private void Update() {
		if (!Game.Balling) return;
		GameTime -= Time.deltaTime;

		Game.MainGameInfo.timeFromStart += Time.deltaTime;
		Debug.Log(Game.MainGameInfo.timeFromStart);
	}

	public void Bonus() {
		GameTime += m_timeBonus;
	}

	public void Penalty() {
		GameTime -= m_timePenalty;
	}
}
