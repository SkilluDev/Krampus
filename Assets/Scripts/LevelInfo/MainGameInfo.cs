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
    [ShowNativeProperty] public State CurrentState { get; private set; }
    public RoomGeneratorBase RoomGenerator => m_roomGenerator;
    [SerializeField] private RoomGeneratorBase m_roomGenerator;

    public NewUIManager UI => m_ui;
    [SerializeField] private NewUIManager m_ui;

    public ShaderManager ShaderManager => m_shaderManager;
    [SerializeField] private ShaderManager m_shaderManager;


    public IReadOnlyList<ChildType> Types => m_types;
    [SerializeField] private ChildType[] m_types;

    public Krampus Krampus => m_krampus;
    [SerializeField] private Krampus m_krampus;

    public GlobalEvents GlobalEvents => m_globalEvents;
    [SerializeField] private GlobalEvents m_globalEvents;

    public float ComboGainFromChildren => m_comboGainFromChildren;
    [BoxGroup("Combo")] [SerializeField] private float m_comboGainFromChildren;
    public float MaxComboPoints => m_maxComboPoints;
    [BoxGroup("Combo")][SerializeField] private float m_maxComboPoints =100.0f;





    public ChildType NiceChildType { get; set; }

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
    [SerializeField] private bool m_skipIntro = false;

    [SerializeField] private Timer m_timer;
    public Timer Timer => m_timer;

    public float timeFromStart = 0;


    private float m_score;

    public float Score => m_score;



    [SerializeField] private OutroScript m_outro;

    public new enum State {
        Intro,
        WaitingToStart,
        Game,
        Paused,
        Over,
        Won
    }

    public bool Ended => CurrentState == State.Over || CurrentState == State.Won;

    public bool Ballin => CurrentState == State.Game;

    public void SetState(State state) {
        CurrentState = state;
    }
    [NaughtyAttributes.Button("Press To Win")]
    public void DebugWinButton() {
        ProcessEndGame(Ending.Win);
        //Krampus.Kramp.Kontroller.KrampTermination(Ending.Win);
    }

    private IEnumerator Start() {
        yield return new WaitUntil(() => Game.RoomGenInfo != null);

        switch (Game.RoomGenInfo.Regenerate) {
            case RoomGenerationType.First:
                Game.RoomGenInfo.SetInitialSeed();
                break;
            case RoomGenerationType.New:
                Game.RoomGenInfo.SetNewSeed();
                break;
            case RoomGenerationType.Old:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Random.InitState(Game.RoomGenInfo.Seed);
        Game.MainGameInfo.UI.SetSeed(Game.RoomGenInfo.Seed);

        CurrentState = State.Intro;
        NiceChildType = Types[0];
        UI.SetChildrenIcon(Types[1]);
    }

    private void Ready() {

        Krampus.Stats.items = Game.PogMan.m_KrampusItems;

        

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

    public void RegisterNun(Nun nun) {
        m_nunRegistry.Add(nun);
    }

    public void UnregisterNun(Nun nun) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(nun)))
            r.RemoveCharacter(nun);
        m_nunRegistry.Remove(nun);
    }



    private void Update() {
        if (!Game.MainGameInfo.NaughtyChildren.Any() && Game.Balling) {
            Game.MainGameInfo.ProcessEndGame(Ending.Win);
        }

        if (Game.MainGameInfo.Timer.GameTime < 0 && Game.Balling && Krampus.Tongue.CurrentState == KrampusTongue.State.Idle) {
            Krampus.Kontroller.KrampTermination(Ending.LoseTime);
            return;
        }
    }

    public void ProcessEndGame(Ending ending) {
        switch (ending) {
            case Ending.Win:
                Game.MainGameInfo.SetState(State.Won);
                m_outro.PlayOutro();
                break;
            case Ending.LoseNun:
                Game.MainGameInfo.SetState(State.Over);
                break;
            case Ending.LoseTime:
                Game.MainGameInfo.SetState(State.Over);
                break;
        }
        Game.MusicMan.StopMusic();
        StartCoroutine(UI.ProcessEnding(ending));
    }
}
