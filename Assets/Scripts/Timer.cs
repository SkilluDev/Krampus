using LitMotion;
using NaughtyAttributes;
using UnityEngine;

public class Timer : MonoBehaviour {
	[BoxGroup("Timer")][SerializeField] private int m_timeBonus;
	[BoxGroup("Timer")][SerializeField] private int m_timePenalty;

	[ShowNativeProperty] public float GameTime { get; private set; } = 60f;
	[SerializeField] private float m_changeDuration;
	[SerializeField] private float m_lowTime = 10;

	public bool IsLowTime => GameTime < m_lowTime;

	private int m_lastDigit;

	private void OnChildEaten(Krampus krampus, Child child) {
		if (child.IsNaughty) {
			Bonus();
		} else {
			Penalty();
		}
	}
	public void Ready() {
		GameTime = Game.PogMan.GetCurrentLevelStats().Timer;
		if (GameTime <= 0) {
			GameTime = float.MaxValue;
		}

		Game.GlobalEvents.onChildEaten.AddListener(OnChildEaten);
	}

	private void Update() {
		if (!Game.Balling) return;
		GameTime -= Time.deltaTime;
		Game.MainGameInfo.timeFromStart += Time.deltaTime;

		if (!IsLowTime) return;
		int lastDigit = Mathf.FloorToInt(GameTime) % 10;
		if (lastDigit != m_lastDigit) {
			Game.MainGameInfo.UI.TimerDisplay.Popup();
			Game.MainGameInfo.UI.PopupTimer();
			m_lastDigit = lastDigit;
		}
	}

	public void Bonus() {
		//GameTime += m_timeBonus;
		LMotion.Create(GameTime, GameTime + m_timeBonus, m_changeDuration).WithEase(Ease.OutCirc).Bind(f => GameTime = f);
	}

	public void Penalty() {
		GameTime -= m_timePenalty;
	}

	public void SetGameTime(float timer) {
		GameTime = timer;
	 }
}
