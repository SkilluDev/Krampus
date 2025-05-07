using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.SceneManagement;
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




    [Header("Timer")]
    [SerializeField] private int startTime;
    public float timer = 0f;
    [SerializeField] private int timeBonus;
    [SerializeField] private int timePenalty;





    private void Awake() {
        GoodChildIndex = Random.Range(0, Types.Count);
        timer = startTime;
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
            r.RemoveNPC(child);
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
            r.RemoveNPC(nun);
        m_nunRegistry.Remove(nun);
    }

    //===============================================================================

    private void Update() {
        timer -= Time.deltaTime;
    }

    public void Bonus() {
        timer += timeBonus;
    }

    public void Penalty() {
        timer -= timePenalty;
    }
}
