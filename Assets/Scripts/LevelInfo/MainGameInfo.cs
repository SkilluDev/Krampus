using System;
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
    [ShowNativeProperty] public State CurrentState { get; private set; }
    public RoomGeneratorBase RoomGenerator => m_roomGenerator;
    [SerializeField] private RoomGeneratorBase m_roomGenerator;

    public NewUIManager UI => m_ui;
    [SerializeField] private NewUIManager m_ui;

    public ShaderManager ShaderManager => m_shaderManager;
    [SerializeField] private ShaderManager m_shaderManager;


    //====== CHILDREN COLOR

    public Color GoodChildrenColor => m_goodChildrenColor;
    [SerializeField] private Color m_goodChildrenColor;

    public Color BadChildrenColor => m_badChildrenColor;
    [SerializeField] private Color m_badChildrenColor;

    public IReadOnlyList<ChildType> Types => m_types;
    [SerializeField] private ChildType[] m_types;

    public Krampus Krampus => m_krampus;
    [SerializeField] private Krampus m_krampus;

    public GlobalEvents GlobalEvents => m_globalEvents;
    [SerializeField] private GlobalEvents m_globalEvents;

    public ChildType GoodChildType { get; set; }

    public ChildType RandomBadChildType => Types.First(x=>x!=GoodChildType);

    public IReadOnlyCollection<Child> Children => m_childRegistry;
    public IEnumerable<Child> BadChildren => m_badChildRegistry;
    public IEnumerable<Child> GoodChildren => m_goodChildRegistry;
    private List<Child> m_childRegistry = new List<Child>();
    private List<Child> m_badChildRegistry = new List<Child>();
    private List<Child> m_goodChildRegistry = new List<Child>();

    public int BadChildrenCountOnStart { get; private set; }
    public int GoodChildrenCountOnStart { get; private set; }

    public IReadOnlyCollection<Nun> Nuns => m_nunRegistry;
    private List<Nun> m_nunRegistry = new List<Nun>();
    private Dictionary<Room, RoomData> m_roomdata = new Dictionary<Room, RoomData>();
    [SerializeField] private bool m_skipIntro = false;

    [SerializeField] private Timer m_timer;
    public Timer Timer => m_timer;

    public float timeFromStart = 0;


    private float m_score;
    private int m_navmeshRebakeRequests;

    public float Score => m_score;
    [BoxGroup("Sounds")][SerializeField] private Sex m_badSound;


    public new enum State {
        Intro,
        WaitingToStart,
        Game,
        Paused,
        Over,
        Won
    }

    public bool Ballin => CurrentState == State.Game;

    public void SetState(State state) {
        CurrentState = state;
    }
    [NaughtyAttributes.Button("Press To Win")]
    public void DebugWinButton() {
		ProcessEndGame(Ending.Win);
        //Krampus.Kramp.Kontroller.KrampTermination(Ending.Win);
    }

    private void Awake() {
        CurrentState = State.Intro;
        GoodChildType = Types.NullIfEmpty()?.UnityRandomElement();
        UI.SetChildrenIcon(GoodChildType.uiIcon);
    }

    private void Ready() {
        if (m_skipIntro) {
            SetState(State.Game);
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
        if (child.Type == GoodChildType) {
            m_goodChildRegistry.Add(child);
            GoodChildrenCountOnStart += 1;
        } else {
            m_badChildRegistry.Add(child);
            BadChildrenCountOnStart += 1;
        }
    }

    public void UnregisterChild(Child child) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(child)))
            r.RemoveCharacter(child);
        m_childRegistry.Remove(child);
        if (child.Type == GoodChildType) {
	        m_badSound.Play(child.transform.position);
            m_goodChildRegistry.Remove(child);


        } else {
            m_badChildRegistry.Remove(child);
            m_score++;
        }
    }

    public void RegisterNun(Nun nun) {
        m_nunRegistry.Add(nun);
    }

    public void UnregisterNun(Nun nun) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(nun)))
            r.RemoveCharacter(nun);
        m_nunRegistry.Remove(nun);
    }

    public void RequestNavMeshRebake() {
        m_navmeshRebakeRequests += 1;
    }

    private void Update() {
        if ((m_navmeshRebakeRequests <= 0 || Mathf.FloorToInt(Time.time) % 10 == 0) && m_navmeshRebakeRequests <= 3) {
            return;
        }

        RoomGenerator.NavMeshSurface.BuildNavMesh();
        m_navmeshRebakeRequests = 0;
    }

    public void ProcessEndGame(Ending ending) {
        switch (ending){
            case Ending.Win:
                Game.MainGameInfo.SetState(MainGameInfo.State.Over);
                break;
            case Ending.LoseNun:
                Game.MainGameInfo.SetState(MainGameInfo.State.Over);
                break;
            case Ending.LoseTime:
                Game.MainGameInfo.SetState(MainGameInfo.State.Over);
                break;
        }
        UI.ProcessEnding(ending);
		Game.MusicMan.StopMusic();
    }
}
