using System;
using System.Collections;
using System.Collections.Generic;
using Roomgen;
using UnityEngine;

public class PogMan : MonoBehaviour {

	[SerializeField] private LevelSet m_levelSet;

	[SerializeField] private int m_currentLevel = 0;
	public int CurrentLevel => m_currentLevel;
	private List<Item> m_krampusItems;
	public IReadOnlyList<Item> KrampusItems => m_krampusItems;

	public LevelStats GetCurrentLevelStats() {
		return m_levelSet.LevelStats[m_currentLevel];
	}



	private LevelModifier m_nextLevelModifer;
	public LevelModifier NextLevelModifier => m_nextLevelModifer;

	public void ResetProgress() {
		m_currentLevel = 0;
		m_krampusItems = null;
	}

	// those essentially move the list in and out without copying it and making sure no reference lives too long.
	public void Store(ref List<Item> items) {
		m_krampusItems = items;
		items = null;
	}

	public void Unpack(ref List<Item> items) {
		if (m_krampusItems == null) items = new List<Item>();
		else items = m_krampusItems;
		m_krampusItems = null;
	}

	public void SetNextLevelModifier(LevelModifier lm) {
		m_nextLevelModifer = lm;
	}

	public void LoadNextLevel() {
		m_currentLevel += 1;
		if (m_currentLevel < m_levelSet.LevelStats.Count) {
			Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
			Game.MainGameInfo.SetState(MainGameInfo.State.Game);
			Game.LoadState(Game.State.MainGame);
		} else {
			GoBackToMenu();
		}
	}

	public void LoadFirstLevel(bool newSeed) {
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
		ResetProgress();
		Game.RoomGenInfo.Regenerate = RoomGenerationType.First;
		Game.LoadState(Game.State.MainMenu);
	}
}
