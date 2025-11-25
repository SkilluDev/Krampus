using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Roomgen;
using UnityEngine;

public class RoundInfo : MainGameInfo
{


	public RoundUIManager UI {
		get {
			return (RoundUIManager)m_ui;
		}
	}

	public RoomGeneratorBase RoomGenerator => m_roomGenerator;
	[SerializeField] private RoomGeneratorBase m_roomGenerator;

	public ChildType NiceChildType { get; set; }

	public ChildType NaughtyChildType { get; set; }

	public IReadOnlyList<ChildType> Types => m_types;
	[SerializeField] private ChildType[] m_types;

	public ChildType RandomNaughtyChildType => Types.First(x => x != NiceChildType);
	public IEnumerable<ChildType> NaughtyChildTypes => Types.Where(x => x != NiceChildType);

	public IReadOnlyCollection<Child> Children => m_childRegistry;
	public IEnumerable<Child> NaughtyChildren => m_naughtyChildRegistry;
	public IEnumerable<Child> NiceChildren => m_niceChildRegistry;
	private List<Child> m_childRegistry = new List<Child>();
	private List<Child> m_naughtyChildRegistry = new List<Child>();
	private List<Child> m_niceChildRegistry = new List<Child>();

	public int NaughtyChildrenCountOnStart { get; private set; }
	public int NiceChildrenCountOnStart { get; private set; }

	public IReadOnlyCollection<Nun> Nuns => m_nunRegistry;
	private List<Nun> m_nunRegistry = new List<Nun>();
	private Dictionary<Room, RoomData> m_roomdata = new Dictionary<Room, RoomData>();

	[SerializeField] private Timer m_timer;
	public Timer Timer => m_timer;

	public float timeFromStart = 0;


	[SerializeField] private bool m_useLevelModifer = true;

	private float m_score;

	public float Score => m_score;

    public bool Ended => CurrentState == State.Over || CurrentState == State.Won;

	public bool Won => CurrentState == State.Won;

	public bool Lost => CurrentState == State.Over;

	[SerializeField] protected OutroScript m_outro;

    private IEnumerator Start() {
		yield return new WaitUntil(() => Game.RoomGenInfo != null);

		CurrentState = State.Intro;
		NiceChildType = Types[0];
		NaughtyChildType = Types[1];
		UI.SetChildrenIcon(NaughtyChildType.uiIcon);
	}

    private void Ready() {
		if (m_useLevelModifer && Game.PogMan.NextLevelModifier != null) {
			Game.PogMan.NextLevelModifier.UpdateLevel();
		}
	}

	public RoomData GetRoomData(Room r) {
		if (r == null) return null;
		return m_roomdata[r];
	}

    public void CreateRoomData(Room r) {
		if (m_roomdata.ContainsKey(r)) throw new System.Exception("What the fuck");
		var data = r.gameObject.AddComponent<RoomData>();
		data.Init(r);
		m_roomdata.Add(r, data);
	}

	public void ClearRoomData() {
		m_roomdata.Clear();
	}

	public void RegisterChild(Child child) {
		m_childRegistry.Add(child);
		if (!child.IsNaughty) {
			m_niceChildRegistry.Add(child);
			NiceChildrenCountOnStart += 1;

		} else {
			m_naughtyChildRegistry.Add(child);
			NaughtyChildrenCountOnStart += 1;
		}
	}

	public void UnregisterChild(Child child) {
		foreach (var r in m_roomdata.Values.Where(w => w.Contains(child)))
			r.RemoveCharacter(child);

		m_childRegistry.Remove(child);
		if (!child.IsNaughty) {
			m_niceChildRegistry.Remove(child);
		} else {
			m_naughtyChildRegistry.Remove(child);
			m_score++;
		}
	}

    [NaughtyAttributes.Button("Press To Win")]
	public void DebugWinButton() {
		ProcessEndGame(Ending.Win);
		//Krampus.Kramp.Kontroller.KrampTermination(Ending.Win);
	}

	[NaughtyAttributes.Button("Press To Lose")]
	public void DebugLoseButton() {
		ProcessEndGame(Ending.LoseNun);
		//Krampus.Kramp.Kontroller.KrampTermination(Ending.Win);
	}

	public void RegisterNun(Nun nun) {
		m_nunRegistry.Add(nun);
	}

	public void UnregisterNun(Nun nun) {
		foreach (var r in m_roomdata.Values.Where(w => w.Contains(nun)))
			r.RemoveCharacter(nun);
		m_nunRegistry.Remove(nun);
	}

    private void Update() {
        
		if (!m_naughtyChildRegistry.Any() && Game.Balling) {
			ProcessEndGame(Ending.Win);
		}

		if (m_timer.GameTime < 0 && Game.Balling && Krampus.Tongue.CurrentState == KrampusTongue.State.Idle) {
			Krampus.Kontroller.KrampTermination(Ending.LoseTime);
			return;
		}
	}

    public void ProcessEndGame(Ending ending) {
		if (Game.PogMan.IsTutorial) {
			ProcessTutorialEnd(ending);
			return;
		}
		switch (ending) {
			case Ending.Win:
				Game.MainGameInfo.SetState(State.Won);
				m_outro.PlayOutro();
				Game.MusicMan.StopMusic();
				StartCoroutine(AllowNextLevelAfterSeconds(0.5f));
				break;
			case Ending.LoseNun:
				Game.MusicMan.StopMusic();
				Game.MainGameInfo.SetState(State.Over);
				StartCoroutine(AllowNextLevelAfterSeconds(1f));
				break;
			case Ending.LoseTime:
				Game.MusicMan.StopMusic();
				Game.MainGameInfo.SetState(State.Over);
				StartCoroutine(AllowNextLevelAfterSeconds(1f));

				break;
		}
		StartCoroutine(UI.ProcessEnding(ending));
	}

	private void ProcessTutorialEnd(Ending ending) {
		switch (ending) {
			case Ending.Win:
				Game.MainGameInfo.SetState(State.Won);
				m_outro.PlayOutro();
				StartCoroutine(GoToNextLevelAfterSeconds(3f));
				break;
			case Ending.LoseNun:
				Game.MainGameInfo.SetState(State.Over);
				StartCoroutine(ReloadLevelAfterSeconds(2f));
				break;
			case Ending.LoseTime:
				Game.MainGameInfo.SetState(State.Over);
				StartCoroutine(ReloadLevelAfterSeconds(2f));
				break;
		}
	}
}
