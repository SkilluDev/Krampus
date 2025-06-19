using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.Android;

public class PogMan : MonoBehaviour {

	[SerializeField] private LevelSet m_levelSet;

	[SerializeField] private int m_currentLevel = 0;

	public int LevelCount => m_levelSet.LevelStats.Count;
	public int CurrentLevel => m_currentLevel;

	public bool IsThereNextLevel => m_currentLevel < m_levelSet.LevelStats.Count - 1;
	private List<Item> m_krampusItems;
	public IReadOnlyList<Item> KrampusItems => m_krampusItems;

	private float m_timer;

	[ShowNativeProperty] public float TotalRunTime { get => m_timer; }

	private void FixedUpdate() {
		if (Game.Balling) m_timer += Time.fixedDeltaTime;
	}

	public LevelStats GetCurrentLevelStats() {
		return m_levelSet.LevelStats[m_currentLevel];
	}
	public int GetMaxLevel() {
		return m_levelSet.LevelStats.Count + 1;
	 }

	private bool m_clearItemsOnLoad = false;



	private LevelModifier m_nextLevelModifer;
	public LevelModifier NextLevelModifier => m_nextLevelModifer;

	private bool m_canGoToNextLevel;

	public bool CanGoToNextLevel => m_canGoToNextLevel;

	public void ResetProgress() {
		m_currentLevel = 0;
		m_krampusItems = null;
		m_clearItemsOnLoad = true;
		m_timer = 0;
	}

	// those essentially move the list in and out without copying it and making sure no reference lives too long.
	public void Store(ref List<Item> items) {
		m_krampusItems = items;
		items = null;
	}

	public void Unpack(ref List<Item> items) {
		if (m_clearItemsOnLoad) {
			items = new List<Item>();
			m_clearItemsOnLoad = false;
			return;
		}

		if (m_krampusItems == null) items = new List<Item>();
		else items = m_krampusItems;
		m_krampusItems = null;
	}

	public void SetNextLevelModifier(LevelModifier lm) {
		m_nextLevelModifer = lm;
	}

	public void LoadNextLevel() {
		m_canGoToNextLevel = false;
		if (IsThereNextLevel) {
			m_currentLevel += 1;
			Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
			Game.MainGameInfo.SetState(MainGameInfo.State.Game);
			Game.LoadState(Game.State.MainGame);
		} else {
			GoBackToMenu();
		}
	}

	public void LoadFirstLevel(bool newSeed) {
		m_canGoToNextLevel = false;
		if (newSeed) {
			Game.RoomGenInfo.Regenerate = RoomGenerationType.New;
		} else {
			Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
		}

		Game.MainGameInfo.SetState(MainGameInfo.State.Game);
		Game.PogMan.ResetProgress();
		Game.LoadState(Game.State.MainGame);

	}

	public void GoBackToMenu() {
		m_canGoToNextLevel = false;
		ResetProgress();
		Game.RoomGenInfo.Regenerate = RoomGenerationType.First;
		Game.LoadState(Game.State.MainMenu);
	}

	public void AllowNextLevel() {
		m_canGoToNextLevel = true;
	}

	public void SetLevelSet(LevelSet set) {
		m_levelSet = set;
	 }
}
