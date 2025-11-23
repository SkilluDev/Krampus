using System;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyProgression : MonoBehaviour
{
	[SerializeField] private int m_startingTargetCount = 1;
	[SerializeField] private float m_startingTime = 60;
	[SerializeField] private float m_startingSpeed = 12;

	[SerializeField] private float m_difficultyMultiplier = 1.1f;
	private int m_targetCount;
	private float m_time;

	//NOT USED ANYWHERE
	private float m_speed;

	public float CurrentDifficulty => m_targetCount/m_startingTargetCount / (m_time / m_startingTime / (1/(m_speed / m_startingSpeed)));

	public enum DifficultyTransition {
        ATargetUp,
		BTargetUpSpeedUp,
		CTargetUpTimeDown,
		DTimeDown,
		ETimeDownSpeedUp
    }

	private List<DifficultyTransition> m_transitionSequence = new List<DifficultyTransition>()
	{
		DifficultyTransition.BTargetUpSpeedUp,
		DifficultyTransition.ATargetUp,
		DifficultyTransition.ETimeDownSpeedUp,
		DifficultyTransition.DTimeDown,
		DifficultyTransition.CTargetUpTimeDown
	};

	private int m_currentTransitionIndex = 0;

	[SerializeField] private List<LevelStats> m_levelStats = new List<LevelStats>();

	public List<LevelStats> LevelStats => m_levelStats;

	public void Initialize() {
		m_targetCount = m_startingTargetCount;
		m_time = m_startingTime;
		m_speed = m_startingSpeed;
		AddLevelsToLevelSet(100);
	}

	private void AddLevelsToLevelSet(int count) {
        for (int i = 0; i < count; i++) {
			AddNextLevelToLevelSet();
			TransitionToNextDifficulty();
		}

    }

	private void AddNextLevelToLevelSet() {
        LevelStatsWithSpeed stats = new LevelStatsWithSpeed();

    	stats.Initialize(
			naughty: m_targetCount,
			nice: Mathf.RoundToInt(Mathf.Ceil(m_targetCount/2)),
			nun: Mathf.RoundToInt(Mathf.Ceil(m_targetCount/3)),
			width: Mathf.RoundToInt(Mathf.Max(3, Mathf.Round(Mathf.Sqrt(m_targetCount+7)))),
			length: Mathf.RoundToInt(Mathf.Max(3, Mathf.Round(Mathf.Sqrt(m_targetCount+7)))),
			canChoose: false,
			timer: m_time,
			tutorials: null,
			lockWindUp: false,
			speed: m_speed
		);
        m_levelStats.Add(stats);

		Debug.Log(m_levelStats.Count + " New Difficulty - Size:"+Mathf.RoundToInt(Mathf.Max(3, Mathf.Round(Mathf.Sqrt(m_targetCount+7))))+" Target count: " + m_targetCount + ", Time: " + m_time + ", Speed: " + m_speed + ", Difficulty: " + CurrentDifficulty);
	}

	public DifficultyTransition GetNextTransition() {
		DifficultyTransition transition = m_transitionSequence[m_currentTransitionIndex];
		m_currentTransitionIndex = (m_currentTransitionIndex + 1) % m_transitionSequence.Count;
        return transition;
    }


	public void TransitionToNextDifficulty() {
		DifficultyTransition transition = GetNextTransition();
		switch (transition) {
			case DifficultyTransition.ATargetUp:
				m_targetCount += (int)Mathf.Ceil((m_difficultyMultiplier-1)*m_targetCount);
				break;
			case DifficultyTransition.BTargetUpSpeedUp:
				m_targetCount += (int)Mathf.Ceil((m_difficultyMultiplier-1)*m_targetCount);
				m_speed *= m_difficultyMultiplier;
				break;
			case DifficultyTransition.CTargetUpTimeDown:
				m_targetCount += (int)Mathf.Ceil((m_difficultyMultiplier-1)*m_targetCount);
				m_time /= m_difficultyMultiplier;
				break;
			case DifficultyTransition.DTimeDown:
				m_time /= m_difficultyMultiplier;
				break;
			case DifficultyTransition.ETimeDownSpeedUp:
				m_time /= m_difficultyMultiplier;
				m_speed *= m_difficultyMultiplier;
				break;
			default:
				break;
		}
	}

	[NaughtyAttributes.Button]
	private void NextDifficulty() {
		TransitionToNextDifficulty();
	}
}
