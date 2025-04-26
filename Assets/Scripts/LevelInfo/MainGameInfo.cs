using System.Collections.Generic;
using Roomgen;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// De-facto game controller for the main gaame scene
/// </summary>
public class MainGameInfo : LevelInfo {
    public RoomGeneratorBase RoomGenerator => m_roomGenerator;
    [SerializeField] private RoomGeneratorBase m_roomGenerator;

    public IReadOnlyCollection<Child> Children => m_childRegistry;
    private List<Child> m_childRegistry = new List<Child>();

    private Dictionary<Room, RoomData> m_roomdata = new Dictionary<Room, RoomData>();

    public RoomData GetRoomData(Room r) {
        if (r == null) return null;
        return m_roomdata[r];
    }

    public void CreateRoomData(Room r) {
        if (m_roomdata.ContainsKey(r)) throw new System.Exception("What the fuck");
        var data = r.AddComponent<RoomData>();
        data.Init(r);
        m_roomdata.Add(r, data);
    }

    public void RegisterChild(Child child) {
        m_childRegistry.Add(child);
    }

    public void UnregisterChild(Child child) {
        m_childRegistry.Remove(child);
    }
}
