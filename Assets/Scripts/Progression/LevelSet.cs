using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStats {

	[Header("Naughty Kids")]	
	[SerializeField] private int m_naughtyCount;
	public int NaughtyCount => m_naughtyCount;

	[SerializeField] private int m_naughtyJackedCount;
	public int NaughtyJackedCount=> m_naughtyJackedCount;


	[Header("Nice Kids")]
	[SerializeField] private int m_niceCount;
	public int NiceCount => m_niceCount;

	[SerializeField] private int m_niceJackedCount;
	public int NiceJackedCount=>  m_niceJackedCount;




	[Header("Nuns")]
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

	[SerializeField] private TutorialPage m_tutorials;
	public TutorialPage Tutorials => m_tutorials;

	[SerializeField] private bool m_lockWindUpUse = false;

	public bool LockWindUpUse => m_lockWindUpUse;

	public void Initialize(
        int naughty,
        int nice,
        int nun,
        int width,
        int length,
        bool canChoose,
        float timer,
        TutorialPage? tutorials,
        bool lockWindUp)
    {
        m_naughtyCount = naughty;
        m_niceCount = nice;
        m_nunCount = nun;
        m_gridWidth = width;
        m_gridLength = length;
        m_canChooseItems = canChoose;
        m_timer = timer;
        m_tutorials = tutorials ?? TutorialPage.None;
        m_lockWindUpUse = lockWindUp;
    }

    public override string ToString() {
        return $"Naughty: {NaughtyCount}, Nice: {NiceCount}, Nun: {NunCount}, Width: {GridWidth}, Length: {GridLength}, CanChooseItems: {CanChooseItems}, Timer: {Timer}, Tutorials: {Tutorials}, LockWindUpUse: {LockWindUpUse}";
    }
}

public class LevelStatsWithSpeed : LevelStats {
	[SerializeField] private float m_speed = 12f;
	public float Speed => m_speed;

	public void Initialize(
		int naughty,
		int nice,
		int nun,
		int width,
		int length,
		bool canChoose,
		float timer,
		TutorialPage? tutorials,
		bool lockWindUp,
		float speed)
	{
		base.Initialize(
			naughty,
			nice,
			nun,
			width,
			length,
			canChoose,
			timer,
			tutorials,
			lockWindUp
		);
		m_speed = speed;
	}

	public void LogStats() {
		Debug.Log("New Difficulty - Target Count: " + NaughtyCount + ", Time: " + Timer + ", Speed: " + Speed);
    }
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
