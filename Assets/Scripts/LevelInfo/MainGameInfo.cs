using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using Sound;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// De-facto game controller for the main gaame scene
/// </summary>
public class MainGameInfo : LevelInfo {
	[ShowNativeProperty] public State CurrentState { get; protected set; }
	

	public NewUIManager UI => m_ui;
	[SerializeField] private NewUIManager m_ui;

	public ShaderManager ShaderManager => m_shaderManager;
	[SerializeField] private ShaderManager m_shaderManager;

	public DayNightCycle DayNightCycle => m_dayNightCycle;
	[SerializeField] private DayNightCycle m_dayNightCycle;


	

	public Krampus Krampus => m_krampus;
	[SerializeField] private Krampus m_krampus;

	public float WindUpGainFromChildren => m_windUpGainFromChildren;
	[BoxGroup("Wind-up")][SerializeField] private float m_windUpGainFromChildren;
	public float MaxWindUpPoints => m_maxWindUpPoints;
	[BoxGroup("Wind-up")][SerializeField] private float m_maxWindUpPoints = 100.0f;


	[BoxGroup("Item Pool")][SerializeField] private ItemPool m_itemPool;
	public ItemPool ItemPool => m_itemPool;





	

	public new enum State {
		Intro,
		WaitingToStart,
		ItemChoosing,
		Game,
		Paused,
		Over,
		Won,
	}

	

	public bool Ballin => CurrentState == State.Game;

	public void SetState(State state) {
		if (state == State.ItemChoosing) {
			if (Game.PogMan.GetCurrentLevelStats().CanChooseItems) state = State.ItemChoosing;
			else state = State.Game;
		}
		var previous = CurrentState;
		CurrentState = state;
		Game.GlobalEvents.onLevelStateChanged.Invoke(previous, CurrentState);
	}
	

	

	

	

	protected IEnumerator AllowNextLevelAfterSeconds(float time) {
		yield return new WaitForSeconds(time);
		Game.PogMan.AllowNextLevel();
	}

	protected IEnumerator GoToNextLevelAfterSeconds(float time) {
		yield return new WaitForSeconds(time);
		Game.PogMan.LoadNextLevel();
	}

	protected IEnumerator ReloadLevelAfterSeconds(float time) {
		yield return new WaitForSeconds(time);
		Game.PogMan.ReloadCurrentLevel();
	}
}
