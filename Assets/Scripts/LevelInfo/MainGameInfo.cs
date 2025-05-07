using System;
using System.Collections.Generic;
using System.Linq;
using Roomgen;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// De-facto game controller for the main gaame scene
/// </summary>
public class MainGameInfo : LevelInfo {
    [System.Serializable]
    public struct ChildType : IEquatable<ChildType> {
        public Color color;
        public Texture2D shape;

        public bool Equals(ChildType other) => color.Equals(other.color) && Equals(shape, other.shape);

        public override bool Equals(object obj) => obj is ChildType other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(color, shape);
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
    private List<Child> m_childRegistry = new List<Child>();

    public IReadOnlyCollection<Nun> Nuns => m_nunRegistry;


    private List<Nun> m_nunRegistry = new List<Nun>();

    private Dictionary<Room, RoomData> m_roomdata = new Dictionary<Room, RoomData>();

    public IReadOnlyCollection<Child> BadChildren => m_childRegistry.Where(c=>!c.Type.Equals(Types[GoodChildIndex])).ToList();
    public IReadOnlyCollection<Child> GoodChildren => m_childRegistry.Where(c=>c.Type.Equals(Types[GoodChildIndex])).ToList();

    private void Awake() {
        GoodChildIndex = Random.Range(0, Types.Count);
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
    }

    public void UnregisterChild(Child child) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(child)))
            r.RemoveNPC(child);
        m_childRegistry.Remove(child);
    }

    public void RegisterNun(Nun nun) {
        m_nunRegistry.Add(nun);
    }

    public void UnregisterNun(Nun nun) {
        foreach (var r in m_roomdata.Values.Where(w => w.Contains(nun)))
            r.RemoveNPC(nun);
        m_nunRegistry.Remove(nun);
    }
}
