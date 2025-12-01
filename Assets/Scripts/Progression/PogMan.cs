using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;


public class PogMan : MonoBehaviour {

	public enum Difficulty {
		Normal,
		Tutorial,
		Hard,
		OnlyNuns
	}

	private Difficulty m_currentDifficulty;
	public Difficulty CurrentDifficulty => m_currentDifficulty;

	private Task m_task;
	public Task CurrentTask => m_task;

	private Difficulty m_difficultyAfterTutorial;


	[Serializable]
	public class LevelSetForDifficulty : ValueConnectedToEnum<Difficulty> {
		[SerializeField] private LevelSet m_levelSet;
		public LevelSet LevelSet => m_levelSet;
	}

	[SerializeField] private SerializedEnumDictionary<Difficulty, LevelSetForDifficulty> m_levelSets;


	[SerializeField] private LevelSet m_levelSet;

	[SerializeField] private bool m_arcadeMode = true;

	[SerializeField] private DifficultyProgression m_arcadeDifficultyProgression;

	public List<LevelStats> LevelStats => m_arcadeMode ? m_arcadeDifficultyProgression.LevelStats : m_levelSet.LevelStats;

	private int m_currentLevel = 0;

	[SerializeField] private int m_startingLevel = 0;

	private bool m_isTutorial = false;

	public bool IsTutorial => m_isTutorial;

	public int LevelCount => LevelStats.Count;
	public int CurrentLevel => m_currentLevel;

	public bool IsThereNextLevel => m_currentLevel < LevelStats.Count - 1;
	public List<Item> krampusItems;

	public List<Item> m_allKrampusItems;
	public IReadOnlyList<Item> KrampusItems => krampusItems;

	private float m_timer;

	[Header("")]
	[SerializeField] private  int m_goldAmount;
	public int GoldAmount => m_goldAmount;

	[SerializeField] private int m_crystalAmount;
	public int CrystalAmount => m_crystalAmount;



	[ShowNativeProperty] public float TotalRunTime { get => m_timer; }

	private void Ready() {
		if (Game.BootFromMainGame) {
			Game.LoadState(Game.State.MainMenu);
		} else {
			Game.GlobalEvents.onSetManChange.AddListener(OnSetManChange);
		}
	}
	private void Unready() {
		Game.GlobalEvents.onSetManChange.RemoveListener(OnSetManChange);
	}
	private void FixedUpdate() {
		if (Game.Balling) m_timer += Time.fixedDeltaTime;
	}

	public LevelStats GetCurrentLevelStats() {
		if(m_arcadeMode && m_arcadeDifficultyProgression.LevelStats.Count == 0)
			m_arcadeDifficultyProgression.Initialize();
		return LevelStats[m_currentLevel];
	}
	public int GetMaxLevel() {
		return LevelStats.Count;
	}

	private bool m_clearItemsOnLoad = false;
	private LevelModifier m_nextLevelModifer;
	public LevelModifier NextLevelModifier => m_nextLevelModifer;

	private bool m_canGoToNextLevel;

	public bool CanGoToNextLevel => m_canGoToNextLevel;

	public void ResetProgress() {
		m_canGoToNextLevel = false;
		m_currentLevel = m_startingLevel;
		//			m_krampusItems.Clear();
		
		ResetItems();
		//m_clearItemsOnLoad = true;
		m_timer = 0;

	}

	public void ResetItems() {
        krampusItems = null;
    }

	// those essentially move the list in and out without copying it and making sure no reference lives too long.
	public void Store(List<Item> items) {
		krampusItems = new List<Item>(items);
	}

	public void Unpack(ref List<Item> items) {
		if (m_clearItemsOnLoad) {
			items = new List<Item>();
			m_clearItemsOnLoad = false;
			return;
		}

		if (krampusItems == null) items = new List<Item>();
		else items = krampusItems;
		krampusItems = null;
	}


	public void SetNextLevelModifier(LevelModifier lm) {
		m_nextLevelModifer = lm;
	}

	public void LoadNextLevel() {
		m_canGoToNextLevel = false;
		if (IsThereNextLevel) {
			m_currentLevel += 1;
			if(m_arcadeMode) (LevelStats[m_currentLevel] as LevelStatsWithSpeed).LogStats();
			SetNextSeed();
			Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
			Game.LoadState(Game.State.MainGame);
		} else {
			if (IsTutorial) {
				m_isTutorial = false;
				Game.SetMan.SetValue<bool>("Show Tutorial", false);
				StartNewGame(m_difficultyAfterTutorial);
			} else {
				//TODO add a game completed screen
				GoBackToLobby();
			}
		}
	}

	public void LoadFirstLevel(bool newSeed) {
		m_canGoToNextLevel = false;
		if (newSeed) {
			Game.RoomGenInfo.Regenerate = RoomGenerationType.New;
		} else {
			Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
		}
		StartNewGame(m_levelSet);
	}

	public void ReloadCurrentLevel() {
		Game.LoadState(Game.State.MainGame);
	}

	public void GoBackToMenu() {
		m_canGoToNextLevel = false;
		ResetProgress();
		Game.RoomGenInfo.Regenerate = RoomGenerationType.First;
		Game.LoadState(Game.State.MainMenu);
	}
	public void GoBackToLobby() {
		m_canGoToNextLevel = false;
		ResetProgress();
		Game.RoomGenInfo.Regenerate = RoomGenerationType.First;
		Game.LoadState(Game.State.Lobby);
	}

	public void AllowNextLevel() {
		m_canGoToNextLevel = true;
	}

	public void SetLevelSet(Difficulty difficulty) {
		m_levelSet = m_levelSets[difficulty].LevelSet;
		m_currentDifficulty = difficulty;
	}

	public void SetSeed() {
		switch (Game.RoomGenInfo.Regenerate) {
			case RoomGenerationType.First:
				Game.RoomGenInfo.SetInitialSeed();
				break;
			case RoomGenerationType.New:
				Game.RoomGenInfo.SetInitialSeed();
				break;
			case RoomGenerationType.Old:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		SetNextSeed();
	}

	public void SetNextSeed() {
		Random.InitState(Game.RoomGenInfo.Seed + CurrentLevel);
	}

	public void StartNewGame(Difficulty difficulty) {
		if (Game.SetMan.GetValue<bool>("Show Tutorial")) {
			m_isTutorial = true;
			m_difficultyAfterTutorial = difficulty;
			SetLevelSet(Difficulty.Tutorial);
		} else {
			m_isTutorial = false;
			m_difficultyAfterTutorial = difficulty;
			SetLevelSet(difficulty);
		}
		ResetProgress();
		SetSeed();
		Game.LoadState(Game.State.MainGame);
	}

	public void StartNewGame(LevelSet levelSet) {
		m_levelSet = levelSet;
		
		ResetProgress();
		SetSeed();
		m_arcadeMode = false;
		Game.LoadState(Game.State.MainGame);
	}

	public void StartNewGame(Task task) {
		m_task = task;
		m_levelSet = task.levelSet;
		
		ResetProgress();
		SetSeed();
		m_arcadeMode = false;
		Game.LoadState(Game.State.MainGame);
	}


	private void OnSetManChange(string key) {
		if (key == "Seed Override") {
			Game.RoomGenInfo.SetInitialSeed();
		}
	}

	//Things related to gold

	public void AddGold(int value) {m_goldAmount += value;}

	public bool HasGold(int value){ return GoldAmount >= value;}

	public void PayGold(int value){m_goldAmount -= value;}

	public void PayOff() {
        AddGold(m_task.goldAmount);
    }


	//All items Unlock Items


	public void UnlockItem(Item item) {
		if(!m_allKrampusItems.Contains(item)){
       	 m_allKrampusItems.Add(item);
		}
    }

}
