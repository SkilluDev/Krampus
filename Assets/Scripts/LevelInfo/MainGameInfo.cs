using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// De-facto game controller for the main gaame scene
/// </summary>
public class MainGameInfo : LevelInfo {
    [System.Serializable]
    public struct ChildType {
	    public int id;
        public Color color;
        public Texture2D shape;
    }

    public RoomGeneratorBase RoomGenerator => m_roomGenerator;
    [SerializeField] private RoomGeneratorBase m_roomGenerator;

    public NewUIManager UI => m_ui;
    [SerializeField] private NewUIManager m_ui;



    public IReadOnlyList<ChildType> Types => m_types;
    [SerializeField] private ChildType[] m_types;

    public Krampus Krampus => m_krampus;
    [SerializeField] private Krampus m_krampus;

    public int GoodChildIndex { get; private set; }

    public IReadOnlyCollection<Child> Children => m_childRegistry;
    public IEnumerable<Child> BadChildren => m_badChildRegistry;
    public IEnumerable<Child> GoodChildren => m_goodChildRegistry;
    private List<Child> m_childRegistry = new List<Child>();
    private List<Child> m_badChildRegistry = new List<Child>();
    private List<Child> m_goodChildRegistry = new List<Child>();

    public IReadOnlyCollection<Nun> Nuns => m_nunRegistry;
    private List<Nun> m_nunRegistry = new List<Nun>();

    private Dictionary<Room, RoomData> m_roomdata = new Dictionary<Room, RoomData>();



    [ShowNativeProperty] public float Timer { get; private set; }


    [BoxGroup("Timer")][SerializeField][FormerlySerializedAs("timeBonus")] private int m_timeBonus;
    [BoxGroup("Timer")][SerializeField][FormerlySerializedAs("timePenalty")] private int m_timePenalty;


    private void Awake() {
        GoodChildIndex = Random.Range(0, Types.Count);
        Timer = Game.SetMan.GetValue<int>("Timer");
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
        if (child.Type.id == GoodChildIndex) {
	        m_goodChildRegistry.Add(child);
        } else {
	        m_badChildRegistry.Add(child);
        }
    }

    public void UnregisterChild(Child child) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(child)))
            r.RemoveCharacter(child);
        m_childRegistry.Remove(child);
        if (child.Type.id == GoodChildIndex) {
	        m_goodChildRegistry.Remove(child);
        } else {
	        m_badChildRegistry.Remove(child);
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
        Timer -= Time.deltaTime;
    }

    public void Bonus() {
        Timer += m_timeBonus;
    }

    public void Penalty() {
        Timer -= m_timePenalty;
    }

    //======================================================================
}
