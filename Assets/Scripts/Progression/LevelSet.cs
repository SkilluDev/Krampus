using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStats {

	[SerializeField] private int m_naughtyCount;
	public int NaughtyCount => m_naughtyCount;

	[SerializeField] private int m_niceCount;
	public int NiceCount => m_niceCount;

	[SerializeField] private int m_nunCount;
	public int NunCount => m_nunCount;

	[SerializeField] private int m_gridWidth;
	public int GridWidth => m_gridWidth;

	[SerializeField] private int m_gridLength;
	public int GridLength => m_gridLength;

	[SerializeField] private bool m_canChooseItems = true;
	public bool CanChooseItems => m_canChooseItems;

	[SerializeField] private float m_timer = 60f;
	public float Timer => m_timer;
}

[CreateAssetMenu(menuName = "Game/LevelProgression/Stats", fileName = "LevelProgressionStats")]
public class LevelSet : ScriptableObject {
	[SerializeField] protected string m_name = "Level Set name";
	public string Name => m_name;

	[TextArea(4, 25)]
	[SerializeField] private string m_description;
	public string Description => m_description;

	[SerializeField] private List<LevelStats> m_levelStats = new List<LevelStats>();

	public List<LevelStats> LevelStats => m_levelStats;
}
