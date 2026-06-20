using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LitMotion;
using LitMotion.Extensions;
using SaintsField;
using SaintsField.Playa;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Playables;
public enum Ending {
	Win,
	LoseNun,
	LoseTime
}

public static class EndingExtension {
	public static bool IsWin(this Ending ending) {
		if (ending == Ending.Win) {
			return true;
		} else {
			return false;
		}
	}
}
public class EndScreenHandler : MonoBehaviour {

	[SerializeField] private float m_fadeDuration = 0.5f;
	[SerializeField] private float m_beginSpeed = 1f;
	[SerializeField] private ChildMeter m_childMeter;


	[Layout("Ending playables", ELayout.FoldoutBox)][SerializeField] private PlayableDirector m_wonDirector, m_lostDirector;
	[Layout("Ending texts", ELayout.FoldoutBox)][SerializeField] private string m_winText;
	[Layout("Ending texts", ELayout.FoldoutBox)][SerializeField] private string m_loseNunText;
	[Layout("Ending texts", ELayout.FoldoutBox)][SerializeField] private string m_loseTimeText;

	public void PreActivate(Ending ending) {
		switch (ending) {
			case Ending.Win:
				Game.MainGameInfo.UI.BlackBars.SetTopBarText(m_winText);
				break;
			case Ending.LoseNun:
				Game.MainGameInfo.UI.BlackBars.SetTopBarText(m_loseNunText);
				break;
			case Ending.LoseTime:
				Game.MainGameInfo.UI.BlackBars.SetTopBarText(m_loseTimeText);
				break;
		}

	}

	public void Activate(Ending ending) {
		gameObject.SetActive(true);
		switch (ending) {
			case Ending.Win:
				m_wonDirector.gameObject.SetActive(true);
				m_wonDirector.Play();

				float time = Game.MainGameInfo.timeFromStart / 60;
				float val = Game.MainGameInfo.Score / time;
				LMotion.Create(0, val, 2).WithEase(Ease.OutElastic).WithDelay(0.5f).Bind(m_childMeter.SetScore);
				break;
			case Ending.LoseNun:
				m_lostDirector.gameObject.SetActive(true);
				m_lostDirector.Play();
				break;
			case Ending.LoseTime:
				m_lostDirector.gameObject.SetActive(true);
				m_lostDirector.Play();
				break;
		}
	}
}
